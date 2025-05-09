using System.Collections.Generic;

[System.Serializable]
public class SaveLoadData
{
    public int gold;                         // 金币数量
    public List<string> ownedSkillIDs;       // 已购技能名列表
    public List<string> equippedSkillIDs;    // 已装备技能名列表
}
