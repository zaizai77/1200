using UnityEngine;
using System.Collections.Generic;
using System;

public class SkillManager : MonoBehaviour
{
    public static SkillManager Instance { get; private set; }

    private Dictionary<SkillSO, ISkillBehavior> behaviorCache = new();

    // —— 新增：冷却字典 ——
    private Dictionary<SkillSO, float> cooldowns = new();

    /// <summary>
    /// 技能冷却变化回调：参数 (skill, remainingCooldown)
    /// </summary>
    public event Action<SkillSO, float> OnCooldownTick;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void OnEnable()
    {
        EventHandler.AfterSceneLoad += OnAfterSceneLoad;
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoad -= OnAfterSceneLoad;
    }

    private void OnAfterSceneLoad()
    {
        // 初始化所有技能的冷却为 0
        foreach (var entry in behaviorCache)
            cooldowns[entry.Key] = 0f;
    }

    void Update()
    {
        // 遍历所有正在冷却的技能，递减计时
        var keys = new List<SkillSO>(cooldowns.Keys);
        foreach (var skill in keys)
        {
            if (cooldowns[skill] > 0f)
            {
                cooldowns[skill] -= Time.deltaTime;
                if (cooldowns[skill] < 0f) cooldowns[skill] = 0f;
                // 广播剩余冷却
                OnCooldownTick?.Invoke(skill, cooldowns[skill]);
            }
        }
    }

    /// <summary>
    /// 释放技能：传入 SkillSO、释放者和目标
    /// </summary>
    public void CastSkill(SkillSO skill, GameObject caster, Vector3 target)
    {
        if(caster.CompareTag("Player"))
        {
            // 如果还在冷却，直接返回
            if (cooldowns.TryGetValue(skill, out float rem) && rem > 0f)
                return;
        }
        
        // 1. 获取或创建行为实例
        if (!behaviorCache.TryGetValue(skill, out ISkillBehavior beh))
        {
            // 计算一个前向偏移距离，单位可以根据角色半径或实验调参
            float forwardOffset = 1.2f;

            if(caster.CompareTag("Boss"))
            {
                forwardOffset = 2.5f;
            }

            // 假设你有 casterTransform 存着施法者的 Transform
            Vector3 spawnPos = caster.transform.position
                             + caster.transform.forward * forwardOffset
                             ;  // verticalOffset 根据需要可设为 0 或角色身高一半

            GameObject go = Instantiate(
                skill.behaviorPrefab,
                spawnPos,
                Quaternion.LookRotation((target - spawnPos).normalized));
            

            //GameObject go = Instantiate(skill.behaviorPrefab);

            beh = go.GetComponent<ISkillBehavior>();
            beh.Initialize(skill, caster);
            behaviorCache[skill] = beh;

            if (caster.CompareTag("Player"))
            {
                // 确保新技能也在冷却字典中初始化
                if (!cooldowns.ContainsKey(skill))
                    cooldowns[skill] = 0f;
            }
        }
        // 2. 执行技能逻辑
        beh.Initialize(skill, caster);
        beh.Execute(target);

        if (caster.CompareTag("Player"))
        {
            // 3. 重置并广播冷却
            cooldowns[skill] = skill.cooldown;
            OnCooldownTick?.Invoke(skill, cooldowns[skill]);
        }
    }

    /// <summary> 查询某技能剩余冷却 </summary>
    public float GetRemainingCooldown(SkillSO skill)
    {
        if (cooldowns.TryGetValue(skill, out float rem)) return rem;
        return 0f;
    }
}
