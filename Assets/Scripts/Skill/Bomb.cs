using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class Bomb : MonoBehaviour
{
    private float explosionDelay;
    private float explosionRadius;
    private float damage;
    private Vector3 target;
    private float throwSpeed;
    private Rigidbody rb;

    public ParticleSystem explosionEffect;
    public AudioClip explosionSfx;

    public void Initialize(
        Vector3 targetPoint,
        float speed,
        float delay,
        float radius,
        float damageAmount
    )
    {
        target = targetPoint;
        throwSpeed = speed;
        explosionDelay = delay;
        explosionRadius = radius;
        damage = damageAmount;

        rb = GetComponent<Rigidbody>();
        // 计算初速度：抛物线公式简化版
        Vector3 dir = (target - transform.position);
        float distance = new Vector2(dir.x, dir.z).magnitude;
        float vy = throwSpeed;
        float vxz = distance / (2 * throwSpeed / Physics.gravity.magnitude);
        Vector3 velocity = new Vector3(
            dir.x / distance * vxz,
            vy,
            dir.z / distance * vxz
        );
        rb.velocity = velocity;

        // 启动倒计时
        StartCoroutine(ExplosionCountdown());
    }

    private IEnumerator ExplosionCountdown()
    {
        yield return new WaitForSeconds(explosionDelay);
        Explode();
    }

    private void Explode()
    {
        // 1. 播放特效
        if (explosionEffect != null)
        {
            var fx = Instantiate(explosionEffect, transform.position, Quaternion.identity);
            fx.Play();
            Destroy(fx.gameObject, fx.main.duration);
        }
        // 2. 范围伤害
        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (var col in hits)
        {
            var hp = col.GetComponent<PlayerHealth>(); // 你的生命组件
            if (hp != null)
                hp.TakeDamage(damage);
            var fp = col.GetComponent<BossBase>();
            if(fp != null)
            {
                fp.TakeDamage(damage);
            }
        }
        // 3. 播放音效
        if (explosionSfx != null)
            AudioSource.PlayClipAtPoint(explosionSfx, transform.position);

        // 销毁炸弹模型
        Destroy(gameObject);
    }

    // 可选：在 Scene 视图画出爆炸范围
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
