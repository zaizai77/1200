using UnityEngine;
using UnityEngine.UI;
using System;

public class EquipSlot : MonoBehaviour
{
    [Header("UI 引用")]
    public Image iconImage;      // 装备图标
    public Button slotButton;    // 整个槽位按钮
    public GameObject plusSign;  // “+” 号占位
    public Text nameText;        //名字

    public void ClearSlot()
    {
        iconImage.sprite = null;
        nameText.text = "";
    }

    /// <summary>
    /// 在槽位内显示已装备技能图标，隐藏“+”，点击后走 onClickCallback
    /// </summary>
    public void SetSkill(int index, SkillSO skill)
    {
        iconImage.sprite = skill.icon;
        iconImage.enabled = true;
        nameText.text = skill.skillName;
        plusSign.SetActive(false);
    }
}
