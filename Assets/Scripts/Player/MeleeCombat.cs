using UnityEngine;

public class MeleeCombat : MonoBehaviour, ICombat
{
    public float attackRadius = 1.5f;
    public int damage = 20;

    public void Attack()
    {
        Collider[] hits = Physics.OverlapSphere(
            transform.position + transform.forward * attackRadius,
            attackRadius
        );
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                //hit.GetComponent<Health>().TakeDamage(damage);
            }
        }
    }
}
