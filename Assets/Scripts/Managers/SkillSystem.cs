using UnityEngine;
using System;
using System.Collections.Generic;

public class SkillSystem : MonoBehaviour
{
    public static SkillSystem Instance { get; private set; }

    [Header("当前持有的技能(最多4)")]
    public List<SkillSO> equippedSkills = new List<SkillSO>();

    // 冷却计时器，与 equippedSkills 索引一一对应
    private float[] cooldownTimers;

    // 施放技能时的全局事件：参数(idx, orthogonal WorldTarget)
    public event Action<int, Vector3> OnSkillCast;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    void Start()
    {
        // 初始化冷却数组
        cooldownTimers = new float[equippedSkills.Count];
    }

    void Update()
    {
        // 每帧减少所有冷却
        for (int i = 0; i < cooldownTimers.Length; i++)
        {
            if (cooldownTimers[i] > 0f)
                cooldownTimers[i] -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("检测到了数字键按下 外");
            
        }

        // 数字键快速施放
        for (int key = 0; key < equippedSkills.Count && key < 4; key++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + key))
            {
                Debug.Log("检测到了数字键按下 里");
                TryCast(key);
            }
        }
    }

    /// <summary>
    /// 尝试施放第 idx 个技能，失败时无副作用
    /// </summary>
    public bool TryCast(int idx)
    {
        if (idx < 0 || idx >= equippedSkills.Count) return false;
        if (cooldownTimers[idx] > 0f) return false;

        // 得到目标点（可根据游戏需要更换）
        Vector3 target = GetAimPoint();

        // 触发技能执行
        equippedSkills[idx].Execute(transform, target);

        // 启动冷却
        cooldownTimers[idx] = equippedSkills[idx].cooldown;

        // 通知其他系统（如 UI）
        OnSkillCast?.Invoke(idx, target);
        return true;
    }

    /// <summary>
    /// 外部查询 idx 技能当前剩余冷却(秒)
    /// </summary>
    public float GetCooldown(int idx)
    {
        if (idx < 0 || idx >= cooldownTimers.Length) return 0f;
        return Mathf.Max(0f, cooldownTimers[idx]);
    }

    /// <summary>
    /// 获取技能冷却最大值(秒)
    /// </summary>
    public float GetMaxCooldown(int idx)
    {
        if (idx < 0 || idx >= equippedSkills.Count) return 0f;
        return equippedSkills[idx].cooldown;
    }

    Vector3 GetAimPoint()
    {
        // 默认用鼠标点击落点
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hit, 100f))
            return hit.point;
        return transform.position;
    }

    /// <summary>
    /// 装备新技能(保持顺序)，若超过4则忽略
    /// </summary>
    public bool EquipSkill(SkillSO skill)
    {
        if (equippedSkills.Count >= 4 || equippedSkills.Contains(skill))
            return false;
        equippedSkills.Add(skill);
        ResizeCooldownArray();
        return true;
    }

    /// <summary>
    /// 卸下技能
    /// </summary>
    public bool UnequipSkill(SkillSO skill)
    {
        int idx = equippedSkills.IndexOf(skill);
        if (idx < 0) return false;
        equippedSkills.RemoveAt(idx);
        ResizeCooldownArray();
        return true;
    }

    // 当 equippedSkills 长度变更时重置冷却数组
    private void ResizeCooldownArray()
    {
        var newTimers = new float[equippedSkills.Count];
        for (int i = 0; i < Mathf.Min(newTimers.Length, cooldownTimers.Length); i++)
            newTimers[i] = cooldownTimers[i];
        cooldownTimers = newTimers;
    }
}
