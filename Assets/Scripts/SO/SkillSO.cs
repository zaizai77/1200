using UnityEngine;

[CreateAssetMenu(fileName = "NewSkill", menuName = "Skills/Skill")]
public class SkillSO : ScriptableObject
{
    [Header("基本属性")]
    public string skillName;                  // 技能名称  
    public Sprite icon;                       // 技能图标  
    public float cooldown = 1f;               // 冷却时间（秒）  
    public float speed = 2f;
    public int price;                         //价格

    [Header("效果与数值")]
    public GameObject effectPrefab;           // 技能特效预制  
    public float castRange = 5f;              // 施法范围  
    public int damage = 10;                   // 伤害数值  
    public AudioClip hitSound;             // 命中音效

    [Header("Behavior")]
    public GameObject behaviorPrefab; // 包含 ISkillBehavior 组件的预制体

    [Header("技能描述")]
    public string description;

    /// <summary>
    /// 执行技能逻辑：实例化特效并在指定目标位置触发效果  
    /// </summary>
    public void Execute(Transform caster, Vector3 target)
    {
        if (effectPrefab != null)
        {
            GameObject eff = Instantiate(
                effectPrefab,
                caster.position + Vector3.up * 1f,
                Quaternion.LookRotation(target - caster.position)
            );
            // 这里可进一步添加伤害判定、AOE、后续效果等  
        }
    }
}
