using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillSOList",menuName = "Skills/SkillList")]
public class SkillSOList : ScriptableObject
{
    public List<SkillSO> skillSOList = new List<SkillSO>();

    public SkillSO FindSkill(string name)
    {
        foreach (var skill in skillSOList)
        {
            if(skill.name == name)
            {
                return skill;
            }
        }
        return null;
    }

}
