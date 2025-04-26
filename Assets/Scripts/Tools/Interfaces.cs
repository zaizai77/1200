// 移动接口
using UnityEngine;



public interface IMovement
{
    void Move(Vector3 direction);
}

// 普通战斗接口
public interface ICombat
{
    void Attack();
}

// 技能释放接口
public interface ISkillCaster
{
    void CastSkill(int skillIndex, Vector3 target);
}
