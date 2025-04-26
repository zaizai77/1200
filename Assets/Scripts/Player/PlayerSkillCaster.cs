using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlayerSkillCaster : MonoBehaviour, ISkillCaster
{
    [Header("技能配置")]
    public SkillSO[] skills = new SkillSO[4];   // 绑定至 Inspector 的技能列表
    private float[] cooldownTimers;

    void Start()
    {
        cooldownTimers = new float[skills.Length];
    }

    void Update()
    {
        // 更新冷却计时器
        for (int i = 0; i < cooldownTimers.Length; i++)
        {
            if (cooldownTimers[i] > 0f)
                cooldownTimers[i] -= Time.deltaTime;
        }

        // 按键触发示例（1-4 键）
        for (int i = 0; i < skills.Length; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i) && cooldownTimers[i] <= 0f)
            {
                Vector3 target = GetMousePoint();
                skills[i].Execute(transform, target);
                cooldownTimers[i] = skills[i].cooldown;
            }
        }
    }

    // 辅助：将鼠标位置转换为世界坐标
    Vector3 GetMousePoint()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out RaycastHit hit);
        return hit.point;
    }

    public void CastSkill(int idx, Vector3 target)
    {
        if (idx < 0 || idx >= skills.Length) return;
        skills[idx].Execute(transform, target);
    }
}
