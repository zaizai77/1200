using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleController : MonoBehaviour
{
    public bool isMovable;
    public Vector3[] pathPoints;
    private int idx;

    void Update()
    {
        if (isMovable) Move();
    }

    void Move()
    {
        // TODO: 在pathPoints间循环移动
    }
}
