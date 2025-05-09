using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0f);
        UIManager.Instance.UpdateHealthBar(currentHealth / maxHealth); // 更新 UI

        if (currentHealth <= 0f)
            Die();
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        UIManager.Instance.UpdateEnemyHealthBar(currentHealth / maxHealth);
    }

    void Die()
    {
        // 播放死亡动画、切换场景等
        Debug.Log("Player has died.");
    }
}
