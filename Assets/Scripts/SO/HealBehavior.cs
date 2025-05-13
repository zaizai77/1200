using UnityEngine;

public class HealBehavior : MonoBehaviour, ISkillBehavior
{
    private SkillSO skillData;
    private GameObject caster;
    private AudioSource audioSource;

    [Header("Optional Effects")]
    public ParticleSystem healEffect;   // 治疗粒子特效
    public AudioClip healSoundClip;     // 治疗音效

    public void Initialize(SkillSO data, GameObject casterObj)
    {
        skillData = data;
        caster = casterObj;
        audioSource = caster.GetComponent<AudioSource>()
                      ?? caster.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    public void Execute(Vector3 target)
    {
        // 1. 播放特效
        if (healEffect != null)
        {
            var effect = Instantiate(healEffect, caster.transform.position, Quaternion.identity);
            effect.Play();
            Destroy(effect.gameObject, effect.main.duration);
        }

        var health = caster.GetComponent<PlayerHealth>();
        if (health != null)
        {
            health.Heal(skillData.damage);
        }

        var bossHeal = caster.GetComponent<BossBase>();
        if(bossHeal != null)
        {
            bossHeal.Heal(skillData.damage);
        }

        // 3. 播放音效
        if (healSoundClip != null)
        {
            audioSource.PlayOneShot(healSoundClip);
        }
    }
}
