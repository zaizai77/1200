using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [Header("战斗中UI")]
    public GameObject battlePanel;
    public Text coinText;
    public Slider playerHealthSlider;
    public Slider playerEnegySlider;
    public Slider enemyHealthSlider;
    public Text countDownText;
    public bool gameOver = true;

    [Header("时间设置")]
    public float totalTime = 120f;    // 倒计时时长，单位秒
    private float remainingTime;      // 剩余时间
    private bool canCountTime = false;

    [Header("技能相关")]
    public PlayerData playerData;
    public List<SkillSO> allSkills;      // 所有技能定义（拖入或从 Resources 加载）
    public Transform skillTrans;
    public GameObject skillBtnPrefab;

    [Header("开始界面")]
    public GameObject startPanel;

    [Header("商店界面")]
    public GameObject storePanel;

    [Header("游戏结束界面")]
    public GameObject gameEndPanel;
    public Text gameOverText;

    // 新增：存储当前技能按钮
    private struct SkillBtnEntry
    {
        public SkillSO skill;
        public Button button;
        public Image cooldownOverlay;
    }
    private List<SkillBtnEntry> skillBtnEntries = new List<SkillBtnEntry>();

    private BossBase boss;

    protected override void Awake()
    {
        base.Awake();
        //boss = GameObject.FindWithTag("Boss").gameObject.GetComponent<BossBase>();
    }

    private void OnEnable()
    {
        EventHandler.AfterSceneLoad += OnAfterSceneLoad;   
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoad -= OnAfterSceneLoad;
    }

    private void Start()
    {
        
    }

    private void OnAfterSceneLoad()
    {
        boss = GameObject.FindWithTag("Boss").gameObject.GetComponent<BossBase>();

        if(SceneManager.GetActiveScene().name == "BattleScene")
        {
            canCountTime = true;
        }
        else
        {
            countDownText.text = "";
            canCountTime = false;
        }

    }

    public void StartGame()
    {
        startPanel.SetActive(false);

        battlePanel.SetActive(true);
        battlePanel.transform.SetAsLastSibling();

        gameOver = false;

        playerEnegySlider.value = 1;
        playerHealthSlider.value = 1;
        enemyHealthSlider.value = 1;

        RefreshSkillBar();
        RestartTimer();
        UpdateUI();
        coinText.text = "Skill Coin:" + playerData.gold;
        TransitionManager.Instance.Transition("BattleScene");
    }

    private void Update()
    {
        if(gameOver)
        {
            return;
        }

        //技能快捷键
        if (Input.GetKeyDown(KeyCode.Alpha1))
            PlayerKeyCode(0);
        if (Input.GetKeyDown(KeyCode.Alpha2))
            PlayerKeyCode(1);
        if (Input.GetKeyDown(KeyCode.Alpha3))
            PlayerKeyCode(2);
        if (Input.GetKeyDown(KeyCode.Alpha4))
            PlayerKeyCode(3);

        // 如果时间已归零，则不再继续计时
        if (remainingTime <= 0f || canCountTime == false) return;

        // 每帧减少 elapsed 时间
        remainingTime -= Time.deltaTime;

        if (remainingTime <= 0f)
        {
            remainingTime = 0f;
            OnTimerEnd();
        }

        UpdateUI();
        UpdateCooldownUI();
    }

    private void UpdateCooldownUI()
    {
        foreach (var entry in skillBtnEntries)
        {
            float rem = SkillManager.Instance.GetRemainingCooldown(entry.skill);
            float max = entry.skill.cooldown;

            // 遮罩填充比例：1 表示完全冷却中（遮满），0 表示可用
            if (entry.cooldownOverlay != null)
                entry.cooldownOverlay.fillAmount = rem / max;

            // 当冷却完毕，按钮可交互，否则禁用
            if (entry.button != null)
                entry.button.interactable = (rem <= 0f);
        }
    }

    private void OnTimerEnd()
    {
        // 倒计时结束时玩家失败逻辑
        Debug.Log("Time's up! Player failed.");
        GameOver(false);
    }

    /// <summary>
    /// 可由外部调用来重置并重新开始计时
    /// </summary>
    public void RestartTimer()
    {
        remainingTime = totalTime;
        UpdateUI();
    }

    private void UpdateUI()
    {
        // 文本：MM:SS 格式
        int minutes = Mathf.FloorToInt(remainingTime / 60f);
        int seconds = Mathf.FloorToInt(remainingTime % 60f);
        countDownText.text = "Time:" + $"{minutes:00}:{seconds:00}";
    }

    public void GameOver(bool isWin)
    {
        gameEndPanel.SetActive(true);
        battlePanel.SetActive(false);
        gameEndPanel.transform.SetAsLastSibling();

        if(isWin)
        {
            gameOverText.text = "You Win!";
        }
        else
        {
            gameOverText.text = "You Lose!";
        }
    }

    public void BackBtn()
    {
        string name = SceneManager.GetActiveScene().name;

        gameEndPanel.SetActive(false);

        startPanel.SetActive(true);
        startPanel.transform.SetAsLastSibling();

        TransitionManager.Instance.UnloadScene(name);
    }

    public void XunLianChang()
    {
        startPanel.SetActive(false);

        battlePanel.SetActive(true);
        battlePanel.transform.SetAsLastSibling();
        RefreshSkillBar();

        playerEnegySlider.value = 1;
        playerHealthSlider.value = 1;
        enemyHealthSlider.value = 1;
        coinText.text = "Skill Coin:" + playerData.gold;
        TransitionManager.Instance.Transition("XunLianChang");
    }

    public void OpenStore()
    {
        startPanel.SetActive(false);

        storePanel.SetActive(true);
        storePanel.transform.SetAsLastSibling();
    }

    public void CloseStorePanel()
    {
        storePanel.SetActive(false);
        storePanel.transform.SetAsFirstSibling();

        startPanel.SetActive(true);
    }

    /// <summary>
    /// 根据 playerData.equippedSkillIDs 生成按钮
    /// </summary>
    public void RefreshSkillBar()
    {
        // 1. 清空旧按钮
        foreach (Transform t in skillTrans)
            Destroy(t.gameObject);

        skillBtnEntries.Clear();

        // 2. 遍历已装备技能列表
        foreach (string skillId in playerData.equippedSkillIDs)
        {
            // 查找对应的 SkillSO
            SkillSO skill = allSkills.Find(s => s.skillName == skillId);
            if (skill == null) continue;

            // 实例化按钮并初始化
            var btnObj = Instantiate(skillBtnPrefab, skillTrans);
            var ui = btnObj.GetComponent<SkillButtonUI>();
            ui.Init(skill, () => OnSkillButtonClicked(skill));

            // 假设 SkillButtonUI 下有一个名为 "CooldownOverlay" 的 Image
            var overlay = btnObj.transform.Find("CooldownOverlay")?.GetComponent<Image>();
            if (overlay == null)
                Debug.LogWarning($"Button for {skill.skillName} needs a child 'CooldownOverlay' Image");

            // 缓存
            skillBtnEntries.Add(new SkillBtnEntry
            {
                skill = skill,
                button = btnObj.GetComponent<Button>(),
                cooldownOverlay = overlay
            });
        }
    }

    private void PlayerKeyCode(int index)
    {
        string skillID = playerData.equippedSkillIDs[index];

        SkillSO skill = allSkills.Find(s => s.skillName == skillID);

        if(skill == null)
        {
            return;
        }

        OnSkillButtonClicked(skill);
    }

    /// <summary>
    /// 按钮点击时调用：释放该技能
    /// </summary>
    void OnSkillButtonClicked(SkillSO skill)
    {
        // 这里调用你的释放接口
        SkillManager.Instance.CastSkill(skill, playerData.gameObject, boss.transform.position);
    }

    public void AddCoin()
    {
        playerData.gold++;
        //刷新UI；
        coinText.text = "Skill Coin:" + playerData.gold;
    }

    public void UpdateHealthBar(float healthValue)
    {
        playerHealthSlider.value = healthValue;
    }

    public void UpdateEnegyBar(float healthValue)
    {
        playerEnegySlider.value = healthValue;
    }

    public void UpdateEnemyHealthBar(float healthValue)
    {
        enemyHealthSlider.value = healthValue;
    }
}
