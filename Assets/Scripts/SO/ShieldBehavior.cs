using UnityEngine;

public class ShieldBehavior : MonoBehaviour, ISkillBehavior
{
    private SkillSO data;
    private GameObject caster;
    public GameObject shieldObj;

    public void Initialize(SkillSO skillData, GameObject casterObj)
    {
        data = skillData;
        caster = casterObj;
    }

    public void Execute(Vector3 target)
    {
        // 1. 实例化护盾特效预制体
        if (data.effectPrefab != null)
        {
            shieldObj = Instantiate(data.effectPrefab, caster.transform);
            shieldObj.transform.localPosition = Vector3.zero;
        }

        // 2. 给 caster 添加 Shield 组件，并初始化参数
        var shield = caster.GetComponent<Shield>();
        if (shield == null)
            shield = caster.AddComponent<Shield>();

        shield.Initialize(data.damage, data.cooldown, shieldObj);
    }
}
