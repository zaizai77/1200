using UnityEngine;

public class FireballBehavior : MonoBehaviour, ISkillBehavior
{
    private SkillSO data;
    private Transform caster;

    public void Initialize(SkillSO skillData, GameObject casterObj)
    {
        data = skillData;
        caster = casterObj.transform;
    }

    public void Execute(Vector3 target)
    {
        // 1. 生成火球预制
        GameObject fb = Instantiate(data.effectPrefab, caster.position + Vector3.up, Quaternion.identity);
        // 2. 设置方向与速度
        Vector3 dir = (target - fb.transform.position).normalized;

        Projectile proj = fb.GetComponent<Projectile>();
        proj.Initialize(data, target);
    }
}
