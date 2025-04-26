using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DynamicObstacle : MonoBehaviour
{
    private NavMeshObstacle obstacle;

    void Awake()
    {
        obstacle = gameObject.AddComponent<NavMeshObstacle>();
        obstacle.carving = true;                   // 启用雕刻
        obstacle.carveOnlyStationary = false;      // 移动也雕刻
        obstacle.size = new Vector3(1, 2, 1);      // 与模型大小匹配
    }
}
