using UnityEngine;

public class Health : MonoBehaviour
{
    private Player player;

    public int maxHealth;
    public int currentHealth;

    public float currentHealthPercentage => (float)currentHealth / (float)maxHealth;

    private void Awake()
    {
        player = GetComponent<Player>();

        currentHealth = maxHealth;
    }

    public void ApplyDamage(int damage)
    {
        currentHealth -= damage;

        CheckHealth();
    }

    private void CheckHealth()
    {
        if (currentHealth <= 0)
        {
            player.SwitchStateTo(Player.PlayerState.Dead);
        }
    }

    public void AddHealth(int health)
    {
        currentHealth += health;

        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
    }
}
