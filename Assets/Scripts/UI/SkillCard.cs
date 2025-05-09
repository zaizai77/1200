using UnityEngine;
using UnityEngine.UI;
using System;

public class SkillCard : MonoBehaviour
{
    public Image icon;
    public Text nameText;
    public Text priceText;
    public Button buyButton;

    private SkillSO skillData;
    private Action<SkillSO> onBuyCallback;

    /// <summary>
    /// 初始化商店卡片
    /// </summary>
    /// <param name="skill">技能数据</param>
    /// <param name="owned">是否已拥有</param>
    /// <param name="onBuy">购买回调</param>
    public void Init(SkillSO skill, bool owned, Action<SkillSO> onBuy)
    {
        skillData = skill;
        onBuyCallback = onBuy;

        icon.sprite = skill.icon;
        nameText.text = skill.skillName;
        priceText.text = skill.price.ToString();

        buyButton.interactable = !owned && onBuyCallback != null;
        buyButton.onClick.RemoveAllListeners();
        if (!owned)
            buyButton.onClick.AddListener(() => onBuyCallback(skillData));
        else
            buyButton.onClick.AddListener(() => Debug.Log($"{skill.skillName} 已拥有"));
    }
}
