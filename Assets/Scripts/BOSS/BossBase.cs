using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(NavMeshAgent))]
public abstract class BossBase : MonoBehaviour
{
    [Header("移动区域设置")]
    public Vector2 areaMin = Vector2.zero;     // 区域左下角坐标
    public Vector2 areaSize = new Vector2(8, 8);// 区域尺寸
    public float moveInterval = 2f;            // 每次换方向间隔
    public float moveSpeed = 3f;               // 移动速度

    [Header("视野与感知")]
    public float viewDistance = 10f;           // 最大视距  
    [Range(0, 180)]
    public float viewAngle = 90f;              // 视野角度  
    public Vector3 eyeOffset = new Vector3(0, 1.5f, 0); // 眼睛偏移  
    public LayerMask sightMask;                // 包含 “Player” 和 “Obstacle” 图层

    [Header("技能配置")]
    public SkillSO[] skills;                   // 装载不同技能的 SO 列表

    [Header("血量")]
    public float maxHealth = 100;
    private float currentHealth;

    protected NavMeshAgent agent;
    private float moveTimer;
    private Vector3 initialPosition;
    private const float AREA_SIZE = 8f;

    // 用于可视化：上次射线检测的命中点与结果
    private Vector3 lastHitPoint;
    private bool lastHitPlayer;

    public float fireTime = 5f;
    private float currentTime = 0f;

    private bool canAttack;
    private GameObject player;

    [Header("冻结")]
    private bool isFrozen = false;
    public ParticleSystem frozenEffect;
    private float frozenTimer = 0f;

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;

        player = GameObject.FindWithTag("Player").gameObject;

        currentHealth = maxHealth;

        initialPosition = transform.position;
    }

    private void OnEnable()
    {
        EventHandler.AfterSceneLoad += OnAfterSceneLoad;
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoad -= OnAfterSceneLoad;
    }

    protected virtual void Start()
    {
        moveTimer = moveInterval;
    }

    protected virtual void Update()
    {
        if(canAttack == false)
        {
            return;
        }

        if (isFrozen)
        {
            frozenTimer -= Time.deltaTime;
            if (frozenTimer <= 0f)
                Unfreeze();
            return;
        }

        HandleMovement();
        HandleSightAndAttack();
    }

    public void Freeze(float duration)
    {
        if (isFrozen) return;
        isFrozen = true;
        frozenEffect.gameObject.SetActive(true);
        frozenEffect.Play();
        frozenTimer = duration; 
    }

    private void Unfreeze()
    {
        isFrozen = false;

        frozenEffect.gameObject.SetActive(false);
        frozenEffect.Stop();
    }

    public void OnAfterSceneLoad()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        if(sceneName == "BattleScene")
        {
            canAttack = true;
            maxHealth = 100;
            currentHealth = maxHealth;
        }
        else if(sceneName == "XunLianChang")
        {
            canAttack = false;
            maxHealth = 500;
            currentHealth = maxHealth;
        }
    }

    public void TakeDamage(float amount)
    {
        var shield = GetComponent<Shield>();
        if (shield != null)
        {
            float leftover = shield.Absorb(amount);
            if (leftover <= 0f) return; // 伤害完全被护盾吸收
            amount = leftover;
        }

        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0f);
        
        UIManager.Instance.UpdateEnemyHealthBar(currentHealth / maxHealth); // 更新 UI

        if (currentHealth <= 0f)
            Die();
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        UIManager.Instance.UpdateEnemyHealthBar(currentHealth / maxHealth);
    }

    private void Die()
    {
        Debug.Log("Boss 死了");
        UIManager.Instance.GameOver(true);
        canAttack = false;
    }

    /// <summary>随机移动：在指定区域内行走</summary>
    private void HandleMovement()
    {
        moveTimer -= Time.deltaTime;
        if (moveTimer <= 0f)
        {
            Vector3 rndTarget = GetClampedPlayerPoint();
            agent.SetDestination(rndTarget);    // 自动寻路
            moveTimer = moveInterval;
        }
    }

    /// <summary>
    /// 获取在 8×8 区域内、且尽量靠近玩家的目标点
    /// </summary>
    private Vector3 GetClampedPlayerPoint()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj == null)
        {
            // 找不到玩家就呆在原地
            return transform.position;
        }

        Vector3 playerPos = playerObj.transform.position;
        float halfSize = AREA_SIZE * 0.5f;

        // 分别对 X 和 Z 做 Clamp，Y 轴保持不变
        float clampedX = Mathf.Clamp(playerPos.x,
                                     initialPosition.x - halfSize,
                                     initialPosition.x + halfSize);

        float clampedZ = Mathf.Clamp(playerPos.z,
                                     initialPosition.z - halfSize,
                                     initialPosition.z + halfSize);

        return new Vector3(clampedX, transform.position.y, clampedZ);
    }

    /// <summary>计算区域内随机点</summary>
    private Vector3 GetRandomPointInArea()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj == null) return new Vector3();
        Transform player = playerObj.transform;
        return new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
    }

    private void HandleSightAndAttack()
    {
        if(currentTime >= fireTime)
        {
            CastSkill();
            currentTime = 0;
        }
        else
        {
            currentTime += Time.deltaTime;
        }
    }

    /// <summary>外部调用：释放第 idx 个技能，目标点 target</summary>
    public virtual void CastSkill()
    {
        int index = Random.Range(0, skills.Length);
        
        SkillManager.Instance.CastSkill(skills[index], gameObject, player.transform.position);
        Debug.Log(skills[index].name);
    }

    /// <summary>
    /// 在 Scene 视图中绘制视野范围与边界射线，帮助调试
    /// </summary>
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 eyePos = transform.position + eyeOffset;
        Gizmos.DrawWireSphere(eyePos, viewDistance);              // 绘制视距圆 :contentReference[oaicite:6]{index=6}

        // 绘制Boss的射线检测
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj == null) return;
        Transform player = playerObj.transform;
        Vector3 toPlayer = (player.position + Vector3.up * 0.5f - eyePos).normalized;
        Gizmos.DrawLine(eyePos, toPlayer);

        // 计算视野两侧方向
        Quaternion leftRot = Quaternion.Euler(0, -viewAngle / 2, 0);
        Quaternion rightRot = Quaternion.Euler(0, viewAngle / 2, 0);
        Vector3 leftDir = leftRot * transform.forward;
        Vector3 rightDir = rightRot * transform.forward;

        Gizmos.DrawLine(eyePos, eyePos + leftDir * viewDistance); // 绘制左侧边界线 :contentReference[oaicite:7]{index=7}
        Gizmos.DrawLine(eyePos, eyePos + rightDir * viewDistance); // 绘制右侧边界线 :contentReference[oaicite:8]{index=8}

        // 2. 射线可视化：命中玩家为绿色，否则为红色
        if (lastHitPoint != Vector3.zero)
        {
            Gizmos.color = lastHitPlayer ? Color.green : Color.red;
            Gizmos.DrawLine(eyePos, lastHitPoint);                    // 可见射线:contentReference[oaicite:12]{index=12}
            Gizmos.DrawSphere(lastHitPoint, 0.1f);                    // 命中点
        }
    }
}
