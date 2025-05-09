using UnityEngine;
using System.Collections.Generic;

public class SkillManager : MonoBehaviour
{
    public static SkillManager Instance { get; private set; }

    private Dictionary<SkillSO, ISkillBehavior> behaviorCache = new();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    /// <summary>
    /// 释放技能：传入 SkillSO、释放者和目标
    /// </summary>
    public void CastSkill(SkillSO skill, GameObject caster, Vector3 target)
    {
        // 1. 获取或创建行为实例
        if (!behaviorCache.TryGetValue(skill, out ISkillBehavior beh))
        {
            GameObject go = Instantiate(skill.behaviorPrefab);
            beh = go.GetComponent<ISkillBehavior>();
            beh.Initialize(skill, caster);
            behaviorCache[skill] = beh;
        }
        // 2. 执行技能逻辑
        beh.Initialize(skill, caster);
        beh.Execute(target);
    }
}
