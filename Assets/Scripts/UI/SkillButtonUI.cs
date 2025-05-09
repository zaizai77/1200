using UnityEngine;
using UnityEngine.UI;
using System;

public class SkillButtonUI : MonoBehaviour
{
    public Image iconImage;      // 拖入按钮上的 Image 组件
    private Button button;
    private Action onClickAction;
    private float coolDownTime;

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
    }
}
