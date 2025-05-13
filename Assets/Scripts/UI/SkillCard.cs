using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

public class SkillCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image icon;
    public Text nameText;
    public Text priceText;
    public Button buyButton;

    private SkillSO skillData;
    private Action<SkillSO> onBuyCallback;

    [Tooltip("要显示/隐藏的子物体")]
    public GameObject childToToggle;

    [Header("详情界面")]
    public Text coolDown;
    public Text description;
    public Text range;
    public Text damega;

    private void Awake()
    {
        childToToggle.SetActive(false);
    }

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

        coolDown.text = "冷却时间：" + skill.cooldown;
        description.text = "作用：" + skill.description;
        range.text = "范围：" + skill.castRange;
        damega.text = "伤害：" + skill.damage;
    }

    // 鼠标进入时调用
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (childToToggle != null)
            childToToggle.SetActive(true);
    }

    // 鼠标离开时调用
    public void OnPointerExit(PointerEventData eventData)
    {
        if (childToToggle != null)
            childToToggle.SetActive(false);
    }
}
