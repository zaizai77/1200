using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

public class SkillButtonUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image iconImage;      // 拖入按钮上的 Image 组件
    private Button button;
    private Action onClickAction;
    private float coolDownTime;
    public GameObject tipGo;
    public Text damageText;
    public Text rangeText;

    void Awake()
    {
        button = GetComponent<Button>();
    }

    /// <summary>
    /// 初始化图标和点击事件
    /// </summary>
    public void Init(SkillSO skill, Action onClick)
    {
        iconImage.sprite = skill.icon;
        coolDownTime = skill.cooldown;
        iconImage.SetNativeSize();

        // 清理旧监听器，注册新回调
        button.onClick.RemoveAllListeners();
        onClickAction = onClick;
        button.onClick.AddListener(() => onClickAction());

        damageText.text = "Damage:" + skill.damage;
        rangeText.text = "Range:" + skill.castRange;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        tipGo.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tipGo.SetActive(false);
    }
}
