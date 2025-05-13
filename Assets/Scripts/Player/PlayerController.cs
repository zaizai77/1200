using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(InputManager), typeof(NavMeshAgent))]
public class PlayerController : MonoBehaviour
{
    private InputManager inputMgr;
    private NavMeshAgent agent;
    private GameObject currentIndicator;

    [Header("冰冻")]
    private bool isFrozen = false;
    private float frozenTimer = 0f;
    public ParticleSystem frozenEffect;

    [Header("指示器设置")]
    public GameObject indicatorPrefab;       // 在 Inspector 中拖入指示器 Prefab
    public float indicatorHeight = 0.05f;
    public float arrivalThreshold = 0.1f;

    void Awake()
    {
        inputMgr = GetComponent<InputManager>();
        agent = GetComponent<NavMeshAgent>();
    }

    void OnEnable()
    {
        inputMgr.OnClickMove += HandleClickMove;     // 订阅点击移动事件
        EventHandler.PlayerMove += OnPlayerMove;
    }

    void OnDisable()
    {
        inputMgr.OnClickMove -= HandleClickMove;
        EventHandler.PlayerMove -= OnPlayerMove;
    }

    private void OnPlayerMove(Vector3 targetPos)
    {
        transform.position = targetPos;
    }


    /// <summary>
    /// 响应地面点击：生成指示器并设置 NavMeshAgent 目标
    /// </summary>
    void HandleClickMove(Vector3 targetPos)
    {
        // 销毁旧指示器
        if (currentIndicator != null)
            Destroy(currentIndicator);

        Debug.Log("HanleClickMove");

        // 在点击位置稍微抬高处实例化指示器
        Vector3 spawnPos = new Vector3(
            targetPos.x,
            targetPos.y + indicatorHeight,
            targetPos.z
        );
        currentIndicator = Instantiate(
            indicatorPrefab,
            spawnPos,
            Quaternion.identity
        );

        // 设置寻路目标，启动自动寻路
        agent.SetDestination(targetPos);
    }

    void Update()
    {
        if (isFrozen)
        {
            frozenTimer -= Time.deltaTime;
            if (frozenTimer <= 0f)
                Unfreeze();

            return;
        }

        // 如果存在指示器，且路径已生成，检测剩余距离
        if (currentIndicator != null && !agent.pathPending)
        {
            // 当 Agent 到达阈值范围内时，销毁指示器&#8203;:contentReference[oaicite:3]{index=3}
            if (agent.remainingDistance <= arrivalThreshold)
            {
                Destroy(currentIndicator);
                currentIndicator = null;
            }
        }
    }

    public void Freeze(float duration)
    {
        if (isFrozen) return;
        isFrozen = true;
        frozenTimer = duration;

        frozenEffect.gameObject.SetActive(true);
        frozenEffect.Play();
    }

    private void Unfreeze()
    {
        isFrozen = false;

        frozenEffect.gameObject.SetActive(false);
        frozenEffect.Stop();
    }
}
