using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData :MonoBehaviour
{
    public int gold;                   // 玩家金币
    public List<string> ownedSkillIDs; // 已购买技能的 ID 列表
    public List<string> equippedSkillIDs; // 当前已装备技能 ID 列表（最多4）

    public void ChangeCoins(int count)
    {
        if(gold + count < 0)
        {
            return;
        }

        gold += count;
        
    }
}

