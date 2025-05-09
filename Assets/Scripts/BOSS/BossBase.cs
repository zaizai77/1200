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

    // 用于可视化：上次射线检测的命中点与结果
    private Vector3 lastHitPoint;
    private bool lastHitPlayer;

    public float fireTime = 5f;
    private float currentTime = 0f;

    private bool canAttack;
    private GameObject player;

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;

        player = GameObject.FindWithTag("Player").gameObject;

        currentHealth = maxHealth;
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

        HandleMovement();
        HandleSightAndAttack();
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
            Vector3 rndTarget = GetRandomPointInArea();
            agent.SetDestination(rndTarget);    // 自动寻路:contentReference[oaicite:3]{index=3}
            moveTimer = moveInterval;
        }
    }

    /// <summary>计算区域内随机点</summary>
    private Vector3 GetRandomPointInArea()
    {
        float x = Random.Range(areaMin.x, areaMin.x + areaSize.x);  // 区间随机:contentReference[oaicite:4]{index=4}
        float z = Random.Range(areaMin.y, areaMin.y + areaSize.y);
        return new Vector3(x, transform.position.y, z);
    }

    /// <summary>
    /// 视野检测 + 射线遮挡检查，视线畅通时触发攻击
    /// </summary>
    private void HandleSightAndAttack()
    {
        // 找到玩家
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj == null) return;
        Transform player = playerObj.transform;

        // 1. 判断玩家是否在视野锥内
        Vector3 eyePos = transform.position + eyeOffset;
        Vector3 toPlayer = (player.position + Vector3.up * 0.5f - eyePos).normalized;
        float angle = Vector3.Angle(transform.forward, toPlayer);  // 计算夹角 :contentReference[oaicite:4]{index=4}
        if (angle > viewAngle * 0.5f ||
            Vector3.Distance(eyePos, player.position) > viewDistance)
        {
            return;
        }

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
