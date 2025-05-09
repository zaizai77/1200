using UnityEngine;

public class BombThrowBehavior : MonoBehaviour, ISkillBehavior
{
    [Header("Bomb Settings")]
    public float spawnHeight = 1.5f;  // 初始高度偏移
    public float throwSpeed = 10f;    // 抛掷的初速度（用于计算抛物线）
    public float explosionDelay = 3f; // 延迟自爆时长
    public float explosionRadius = 5f;// 爆炸半径

    private SkillSO skillData;
    private Transform caster;

    public void Initialize(SkillSO data, GameObject casterObj)
    {
        skillData = data;
        caster = casterObj.transform;
    }

    public void Execute(Vector3 target)
    {
        // 1. 在施法者位置实例化炸弹预制体
        Vector3 spawnPos = caster.position + Vector3.up * spawnHeight;
        GameObject bombObj = GameObject.Instantiate(skillData.effectPrefab, spawnPos, Quaternion.identity);

        // 2. 让炸弹根据抛物线运动到目标
        Bomb bomb = bombObj.GetComponent<Bomb>();
        bomb.Initialize(
            target,
            throwSpeed,
            explosionDelay,
            explosionRadius,
            skillData.damage
        );
    }
}
