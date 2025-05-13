using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceBehavior : MonoBehaviour, ISkillBehavior
{
    private SkillSO data;    // 假设你的 SkillSO 类型已扩展为 FireballSkillSO，包含发射角度等字段
    private Transform caster;
    // 45° 的弧度
    float angleRad = 45f * Mathf.Deg2Rad;
    public float spawnHeight = 1.5f;  // 初始高度偏移

    public void Execute(Vector3 target)
    {
        Vector3 spawnPos = caster.position + Vector3.up * spawnHeight;
        GameObject fb = Instantiate(data.effectPrefab, spawnPos, Quaternion.identity);

        // 2. 计算抛物线初速度
        Rigidbody rb = fb.GetComponent<Rigidbody>();
        if (rb == null) rb = fb.AddComponent<Rigidbody>();
        rb.useGravity = true;

        Vector3 velocity = CalculateLaunchVelocity(spawnPos, target, angleRad);

        // 3. 应用初速度
        rb.velocity = velocity;

        // 4. 如果有额外初始化，比如碰撞伤害、特效持续，可在 Projectile 中处理
        var proj = fb.GetComponent<Projectile>();
        if (proj != null)
            proj.Initialize(data, target);
    }

    public void Initialize(SkillSO skillData, GameObject caster)
    {
        data = skillData; ;
        this.caster = caster.transform;
    }

    /// <summary>
    /// 计算在给定发射角度（弧度）下，从 start 向 end 抛物线发射所需的初速度向量
    /// </summary>
    private Vector3 CalculateLaunchVelocity(Vector3 start, Vector3 end, float angle)
    {
        Vector3 toTarget = end - start;
        Vector3 toTargetXZ = new Vector3(toTarget.x, 0, toTarget.z);

        float y = toTarget.y;
        float xz = toTargetXZ.magnitude;
        float g = Physics.gravity.y;

        // v^2 = (g * xz^2) / (2 * (y - xz * tan(angle)) * cos^2(angle))
        float tanAngle = Mathf.Tan(angle);
        float cosAngle = Mathf.Cos(angle);
        float numerator = -g * xz * xz;
        float denominator = 2 * (y - xz * tanAngle) * cosAngle * cosAngle;
        float v2 = numerator / denominator;
        if (v2 <= 0f) v2 = 0f;  // 无效时发直线
        float v = Mathf.Sqrt(v2);

        // 方向
        Vector3 dirXZ = toTargetXZ.normalized;
        // 构造三维速度向量
        return dirXZ * v * cosAngle + Vector3.up * v * tanAngle;
    }
}
