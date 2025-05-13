using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshObstacle))]
public class MovingObstacle : MonoBehaviour
{
    [Header("轨迹点")]
    public Transform[] waypoints;      // 在 Inspector 中拖入一系列 Transform 作为路径节点
    public float speed = 2f;           // 移动速度

    private int currentIndex = 0;
    private bool forward = true;

    void Update()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        // 1. 计算目标点和方向
        Transform target = waypoints[currentIndex];
        Vector3 dir = (target.position - transform.position).normalized;

        // 2. 移动
        transform.position += dir * speed * Time.deltaTime;

        // 3. 到达后切换下一个节点
        if (Vector3.Distance(transform.position, target.position) < 0.1f)
        {
            if (forward)
            {
                currentIndex++;
                if (currentIndex >= waypoints.Length) { currentIndex = waypoints.Length - 1; forward = false; }
            }
            else
            {
                currentIndex--;
                if (currentIndex < 0) { currentIndex = 0; forward = true; }
            }
        }
    }
}
