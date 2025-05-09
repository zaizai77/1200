using UnityEngine;
using System;
using System.Reflection;

/// <summary>
/// InputModule：事件驱动输入
/// </summary>
public class InputManager : MonoBehaviour
{
    public event Action OnAttack;
    public event Action<int, Vector3> OnSkillCast;
    public event Action<Vector3> OnClickMove;   // 新增：点击移动事件

    void Update()
    {
        // 普通攻击（左键）
        if (Input.GetMouseButtonDown(0))
            OnAttack?.Invoke();

        // 技能释放（右键 + 数字键）
        if (Input.GetMouseButtonDown(1) && TryGetSkillKey(out int idx))
        {
            Vector3 target = GetMouseWorldPoint();
            OnSkillCast?.Invoke(idx, target);
        }
        // 点击地面移动（右键，无数字键）
        else if (Input.GetMouseButtonDown(1))
        {
            Vector3 target = GetMouseWorldPoint();
            OnClickMove?.Invoke(target);
        }
    }

    // 尝试读取 1~4 数字键
    private bool TryGetSkillKey(out int idx)
    {
        for (int i = 1; i <= 4; i++)
        {
            if (Input.GetKey(KeyCode.Alpha0 + i))
            {
                idx = i - 1;
                return true;
            }
        }
        idx = -1;
        return false;
    }

    // 将鼠标屏幕坐标转换为地面世界坐标
    Vector3 GetMouseWorldPoint()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);   //contentReference[oaicite: 0]{ index = 0}
        if (Physics.Raycast(ray, out RaycastHit hit, 100f))     //contentReference[oaicite: 1]{ index = 1}
        return hit.point;
        return Vector3.zero;
    }
}
