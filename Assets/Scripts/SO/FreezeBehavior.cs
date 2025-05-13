using UnityEngine;
using System.Collections;

public class FreezeBehavior : MonoBehaviour, ISkillBehavior
{
    private SkillSO data;
    private Transform caster;

    [Header("特效")]
    public ParticleSystem freezeEffectPrefab;
    [Header("音效")]
    public AudioClip freezeSfx;

    private AudioSource audioSource;

    public void Initialize(SkillSO skillData, GameObject casterObj)
    {
        data = skillData;
        caster = casterObj.transform;
        audioSource = casterObj.GetComponent<AudioSource>()
            ?? casterObj.gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    public void Execute(Vector3 target)
    {
        // 1. 播放特效
        if (freezeEffectPrefab != null)
        {
            var fx = Instantiate(freezeEffectPrefab, target, Quaternion.identity);
            fx.Play();
            Destroy(fx.gameObject, fx.main.duration);
        }

        // 2. 播放音效
        if (freezeSfx != null)
            audioSource.PlayOneShot(freezeSfx);

        //判断是不是玩家发出的

        bool playerFire = false;

        PlayerController playerController = caster.gameObject.GetComponent<PlayerController>();
        if(playerController != null)
        {
            playerFire = true;
        }

        // 3. 伤害/冻结范围
        Collider[] cols = Physics.OverlapSphere(target, data.castRange);
        foreach (var col in cols)
        {
            var enemy = col.GetComponent<BossBase>();
            if (enemy != null && playerFire)
            {
                enemy.Freeze(data.cooldown);
            }

            var player = col.GetComponent<PlayerController>();

            if(player != null && !playerFire)
            {
                //player.Freeze(data.cooldown);
            }
        }
    }
}
