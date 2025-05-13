using UnityEngine;
using System.Collections;

public class Shield : MonoBehaviour
{
    private float remainingStrength;
    private float duration = 5f;
    private GameObject visualEffect;

    /// <summary>
    /// 初始化护盾
    /// </summary>
    public void Initialize(float strength, float durationSeconds, GameObject effectObj)
    {
        remainingStrength = strength;
        duration = durationSeconds;
        visualEffect = effectObj;

        // 开始持续时间计时
        StartCoroutine(ShieldLifetime());
    }

    /// <summary>
    /// 尝试吸收伤害，返回未被吸收的部分
    /// </summary>
    public float Absorb(float damage)
    {
        float leftover = 0f;
        if (damage <= remainingStrength)
        {
            remainingStrength -= damage;
        }
        else
        {
            leftover = damage - remainingStrength;
            remainingStrength = 0f;
            DestroyShield();
        }
        return leftover;
    }

    private IEnumerator ShieldLifetime()
    {
        yield return new WaitForSeconds(duration);
        DestroyShield();
    }

    private void DestroyShield()
    {
        if (visualEffect != null)
            Destroy(visualEffect);
        Destroy(this);
    }
}
