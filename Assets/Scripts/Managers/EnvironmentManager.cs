using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentManager : Singleton<EnvironmentManager>
{
    [Header("Map Settings")]
    public int mapSize = 35;                         // 地图尺寸（单位格数）
    [Header("Grid Settings")]
    public Color gridColor = Color.gray;             // 网格线颜色  
    public float lineWidth = 0.05f;                  // 网格线宽度  
    public float gridHeight = 0.01f;                 // 网格绘制高度  
    public Material lineMaterial;                    // 网格线材质，建议使用Sprites/Default  

    [Header("Obstacle & Coin Prefabs")]
    public GameObject basicObstaclePrefab;
    public GameObject customObstaclePrefab;
    public GameObject movingObstaclePrefab;
    public Transform obstacleParent;
    public GameObject skillCoinPrefab;
    public float coinSpawnInterval = 10f;
    private GameObject currentCoin;

    void Start()
    {
        DrawGrid();                                  // 绘制网格  
        GenerateMap();                               // 随机生成障碍物  
        InvokeRepeating(nameof(SpawnSkillCoin),
                        coinSpawnInterval,
                        coinSpawnInterval);         // 定时生成技能币  
    }

    /// <summary>
    /// 在平面上绘制 mapSize × mapSize 的网格
    /// </summary>
    void DrawGrid()
    {
        GameObject gridParent = new GameObject("GridLayer");
        gridParent.transform.SetParent(transform);
        gridParent.transform.position = new Vector3(0, gridHeight, 0);

        for (int i = 0; i <= mapSize; i++)
        {
            // 垂直线
            CreateLine(
                new Vector3(i, 0, 0),
                new Vector3(i, 0, mapSize),
                gridParent.transform
            );
            // 水平线
            CreateLine(
                new Vector3(0, 0, i),
                new Vector3(mapSize, 0, i),
                gridParent.transform
            );
        }
    }

    /// <summary>
    /// 创建单条网格线段，并配置样式
    /// </summary>
    void CreateLine(Vector3 start, Vector3 end, Transform parent)
    {
        GameObject lineObj = new GameObject("GridLine");
        lineObj.transform.SetParent(parent);

        LineRenderer lr = lineObj.AddComponent<LineRenderer>();
        lr.positionCount = 2;                         // 两端顶点&#8203;:contentReference[oaicite:1]{index=1}
        lr.SetPositions(new Vector3[] {
            start + Vector3.up * gridHeight,
            end   + Vector3.up * gridHeight
        });

        // 样式配置：材质、颜色、宽度&#8203;:contentReference[oaicite:2]{index=2}
        lr.material = lineMaterial != null
                           ? lineMaterial
                           : new Material(Shader.Find("Sprites/Default"));
        lr.startColor = gridColor;                 // 起始颜色  
        lr.endColor = gridColor;                 // 结束颜色  
        lr.startWidth = lineWidth;                 // 起始宽度  
        lr.endWidth = lineWidth;                 // 结束宽度  
        lr.loop = false;                     // 不闭合&#8203;:contentReference[oaicite:3]{index=3}
        lr.useWorldSpace = true;                      // 世界坐标系  
    }

    /// <summary>
    /// 随机生成障碍物（示例占位，需根据需求实现）  
    /// </summary>
    void GenerateMap()
    {
        // TODO: 在 obstacleParent 下随机实例化 basicObstaclePrefab、
        // customObstaclePrefab（高度2层）与 movingObstaclePrefab（可移动）  
    }

    /// <summary>
    /// 周期性生成技能币，地图上一次只存在一个  
    /// </summary>
    void SpawnSkillCoin()
    {
        if (currentCoin != null) return;

        float x = Random.Range(0.5f, mapSize - 0.5f);
        float z = Random.Range(0.5f, mapSize - 0.5f);
        Vector3 pos = new Vector3(x, 0.5f, z);

        currentCoin = Instantiate(skillCoinPrefab, pos, Quaternion.identity);
    }
}
