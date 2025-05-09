using UnityEngine;
using UnityEngine.UI;
using System;

public class OwnedSkillCard : MonoBehaviour
{
    [Header("UI 引用")]
    public Image iconImage;        // 技能图标
    public Text nameText;         // 技能名称
    public Button equipButton;     // “装备” 按钮
    public GameObject equippedTag; // “已装备”标记（例如一个小角标）

    private SkillSO skillData;
    private bool isEquipped;
    private Action<SkillSO> onEquipCallback;

    /// <summary>
    /// 初始化已拥有技能卡片
    /// </summary>
    /// <param name="skill">技能数据</param>
    /// <param name="equipped">是否已装备</param>
    /// <param name="onEquip">当点击装备/取消装备时的回调</param>
    public void Init(SkillSO skill, bool equipped, Action<SkillSO,bool> onEquip)
    {
        skillData = skill;
        isEquipped = equipped;
        iconImage.sprite = skill.icon;
        nameText.text = skill.skillName;
        equippedTag.SetActive(equipped);

        // 清除旧监听
        equipButton.onClick.RemoveAllListeners();
        
        onEquip(skillData, equipped);

        // 根据当前装备状态设置按钮文案与行为
        if (equipped)
        {
            equipButton.GetComponentInChildren<Text>().text = "卸下";
            equipButton.interactable = true;
            equipButton.onClick.AddListener(() =>
            {
                isEquipped = false;
                equippedTag.SetActive(false);
                onEquip(skillData,true);
                Init(skillData, isEquipped, onEquip); // 刷新UI状态
            });
        }
        else
        {
            equipButton.GetComponentInChildren<Text>().text = "装备";
            equipButton.interactable = true;
            equipButton.onClick.AddListener(() =>
            {
                isEquipped = true;
                equippedTag.SetActive(true);
                onEquip(skillData,false);
                Init(skillData, isEquipped, onEquip);
            });
        }
    }
}
