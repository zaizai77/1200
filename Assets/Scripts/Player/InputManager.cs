using UnityEngine;
using System;

/// <summary>
/// InputModule：事件驱动输入
/// </summary>
public class InputManager : MonoBehaviour
{
    public event Action<Vector2> OnMove;
    public event Action OnAttack;
    public event Action<int, Vector3> OnSkillCast;

    void Update()
    {
        // 移动输入（WSAD 或 左摇杆）
        Vector2 move = new Vector2(Input.GetAxis("Horizontal"),
                                   Input.GetAxis("Vertical"));
        if (move.sqrMagnitude > 0.01f) OnMove?.Invoke(move);

        // 普通攻击（左键）
        if (Input.GetMouseButtonDown(0)) OnAttack?.Invoke();

        // 技能释放（数字键 + 右键指向点）
        if (Input.GetMouseButtonDown(1))
        {
            int idx = GetPressedSkillKey(); // 1~4
            Vector3 target = GetMouseWorldPoint();
            OnSkillCast?.Invoke(idx, target);
        }
    }

    int GetPressedSkillKey()
    {
        for (int i = 1; i <= 4; i++)
            if (Input.GetKeyDown(KeyCode.Alpha0 + i)) return i - 1;
        return 0;
    }

    Vector3 GetMouseWorldPoint()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out RaycastHit hit);
        return hit.point;
    }
}
