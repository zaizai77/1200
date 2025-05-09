using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(AudioSource), typeof(Collider))]
public class Projectile : MonoBehaviour
{
    private float speed;
    public float damage;
    private AudioClip hitSound;
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        GetComponent<Collider>().isTrigger = true;
    }

    /// <summary>
    /// 初始化子弹属性并发射
    /// </summary>
    public void Initialize(SkillSO skill, Vector3 target)
    {
        // 从 SO 中读取配置
        speed = skill.speed;
        damage = skill.damage;
        hitSound = skill.hitSound;

        // 设置音频源（可选：也可使用 PlayClipAtPoint）
        AudioSource src = gameObject.AddComponent<AudioSource>();
        src.clip = hitSound;
        src.playOnAwake = false;

        // 计算方向并发射
        Vector3 dir = (target - transform.position).normalized;
        rb.velocity = dir * speed;                                       // 物理飞行

        StartCoroutine(TimeDown());
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 扣血
            var ph = other.GetComponent<PlayerHealth>();
            if (ph != null)
                ph.TakeDamage(damage);                                    // 调用玩家掉血
        }
        else if(other.CompareTag("Boss"))
        {
            var ph = other.GetComponent<BossBase>();
            if(ph != null)
            {
                ph.TakeDamage(damage);
            }
        }

        // 播放音效
        if (hitSound != null)
            GetComponent<AudioSource>().Play();

        // 销毁子弹
        Destroy(gameObject);
    }

    IEnumerator TimeDown()
    {
        yield return new WaitForSeconds(20f);
        Destroy(gameObject);
    }
}
