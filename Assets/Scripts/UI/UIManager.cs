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
    }

    public void StartGame()
    {
        startPanel.SetActive(false);

        battlePanel.SetActive(true);
        battlePanel.transform.SetAsLastSibling();

        gameOver = false;

        RefreshSkillBar();
        RestartTimer();
        UpdateUI();
        TransitionManager.Instance.Transition("BattleScene");
    }

    private void Update()
    {
        if(gameOver)
        {
            return;
        }

        // 如果时间已归零，则不再继续计时
        if (remainingTime <= 0f) return;

        // 每帧减少 elapsed 时间
        remainingTime -= Time.deltaTime;

        if (remainingTime <= 0f)
        {
            remainingTime = 0f;
            OnTimerEnd();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
            PlayerKeyCode(0);
        if (Input.GetKeyDown(KeyCode.Alpha2))
            PlayerKeyCode(1);
        if (Input.GetKeyDown(KeyCode.Alpha3))
            PlayerKeyCode(2);
        if (Input.GetKeyDown(KeyCode.Alpha4))
            PlayerKeyCode(3);

        UpdateUI();
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
        countDownText.text = "倒计时:" + $"{minutes:00}:{seconds:00}";
    }

    public void GameOver(bool isWin)
    {
        gameEndPanel.SetActive(true);
        battlePanel.SetActive(false);
        gameEndPanel.transform.SetAsLastSibling();

        if(isWin)
        {
            gameOverText.text = "你胜利了!";
        }
        else
        {
            gameOverText.text = "你失败了!";
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
        coinText.text = "金币:" + playerData.gold;
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
