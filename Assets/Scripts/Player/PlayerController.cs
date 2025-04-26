using UnityEngine;

[RequireComponent(typeof(InputManager))]
public class PlayerController : MonoBehaviour
{
    private InputManager inputMgr;
    private IMovement movement;
    private ICombat combat;
    private ISkillCaster skillCaster;

    void Awake()
    {
        inputMgr = GetComponent<InputManager>();
        movement = GetComponent<IMovement>();
        combat = GetComponent<ICombat>();
        skillCaster = GetComponent<ISkillCaster>();
    }

    void OnEnable()
    {
        inputMgr.OnMove += HandleMove;
        inputMgr.OnAttack += HandleAttack;
        inputMgr.OnSkillCast += HandleSkillCast;
    }

    void OnDisable()
    {
        inputMgr.OnMove -= HandleMove;
        inputMgr.OnAttack -= HandleAttack;
        inputMgr.OnSkillCast -= HandleSkillCast;
    }

    void HandleMove(Vector2 dir)
    {
        movement?.Move(new Vector3(dir.x, 0, dir.y));
    }

    void HandleAttack()
    {
        combat?.Attack();
    }

    void HandleSkillCast(int idx, Vector3 target)
    {
        skillCaster?.CastSkill(idx, target);
    }
}
