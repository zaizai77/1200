using UnityEngine;

public interface ISkillBehavior
{
    /// <summary>
    /// 初始化技能，注入数据与调用者
    /// </summary>
    void Initialize(SkillSO skillData, GameObject caster);

    /// <summary>
    /// 执行技能：比如生成火球、应用特效等
    /// </summary>
    /// <param name="target">目标位置或对象</param>
    void Execute(Vector3 target);
}
