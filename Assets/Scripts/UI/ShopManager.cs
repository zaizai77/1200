using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    [Header("Skill Data")]
    public SkillSO[] allSkills;               // 所有可售技能 (在 Inspector 中配置)

    [Header("UI References")]
    public Transform shopGrid;                // GridLayoutGroup for shop items
    public Transform ownedGrid;               // GridLayoutGroup for owned items
    public Transform equipGrid;               // GridLayoutGroup or parent for equip slots
    public GameObject skillCardPrefab;        // 购买卡片 Prefab
    public GameObject ownedCardPrefab;        // 已拥有卡片 Prefab
    public GameObject equipSlotPrefab;        // 装备槽 Prefab (含“+”号占位)

    public Text goldText;                     // 显示金币数量

    public PlayerData playerData;

    void Start()
    {
        //LoadPlayerData();                     // 从 PlayerPrefs 读取或初始化
        RefreshAllUI();
    }

    private void OnEnable()
    {
        RefreshAllUI();
    }

    void RefreshAllUI()
    {
        goldText.text = "Skill Coin:" + playerData.gold.ToString();

        RefreshShopUI();
        RefreshOwnedUI();
        RefreshEquipUI();
    }

    void RefreshShopUI()
    {
        // 清空旧条目
        foreach (Transform t in shopGrid) Destroy(t.gameObject);

        // 逐个生成
        foreach (var skill in allSkills)
        {
            var cardObj = Instantiate(skillCardPrefab, shopGrid);
            var card = cardObj.GetComponent<SkillCard>();
            bool owned = playerData.ownedSkillIDs.Contains(skill.name);

            card.Init(skill, owned, OnBuySkill);
        }
    }

    void OnBuySkill(SkillSO skill)
    {
        if (playerData.gold >= skill.price &&
            !playerData.ownedSkillIDs.Contains(skill.name))
        {
            playerData.gold -= skill.price;
            playerData.ownedSkillIDs.Add(skill.name);
            //SavePlayerData();
            RefreshOwnedUI();                  // 更新背包,金币
            goldText.text = playerData.gold.ToString();
        }
    }

    void RefreshOwnedUI()
    {
        foreach (Transform t in ownedGrid) Destroy(t.gameObject);

        foreach (var skillID in playerData.ownedSkillIDs)
        {
            var skill = Array.Find(allSkills, s => s.name == skillID);
            if (skill == null) continue;

            var cardObj = Instantiate(ownedCardPrefab, ownedGrid);
            var ownedCard = cardObj.GetComponent<OwnedSkillCard>();
            // 根据当前技能是否在已装备列表里决定 equipped 状态
            bool equipped = playerData.equippedSkillIDs.Contains(skillID);
            ownedCard.Init(skill, equipped, OnEquipSkill);
        }
    }

    void RefreshEquipUI()
    {
        int currentCount = equipGrid.transform.childCount;

        for (int i = currentCount; i < 4; i++)
        {
            Instantiate(equipSlotPrefab, equipGrid);
        }

        
        // 更新每个槽位
        for (int i = 0; i < equipGrid.childCount; i++)
        {
            var slot = equipGrid.GetChild(i).GetComponent<EquipSlot>();
            if (i < playerData.equippedSkillIDs.Count)
            {
                string id = playerData.equippedSkillIDs[i];
                Debug.Log(id);
                var skill = Array.Find(allSkills, s => s.skillName == id);
                slot.SetSkill(i,skill);
            }
            else
            {
                slot.ClearSlot();
            }
        }
    }

    void OnEquipSkill(SkillSO skill,bool toEquip)
    {

        if (toEquip)
        {
            // 如果未满 4 个且尚未装备，添加到已装备列表
            if (playerData.equippedSkillIDs.Count < 4 &&
                !playerData.equippedSkillIDs.Contains(skill.name))
            {
                playerData.equippedSkillIDs.Add(skill.name);
            }
        }
        else
        {
            // 卸下：从已装备列表中移除
            playerData.equippedSkillIDs.Remove(skill.name);
        }

        RefreshEquipUI();
    }
}
