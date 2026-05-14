using UnityEngine;

/// <summary>
/// Tracks player health and handles taking damage.
/// Setup:
/// 1. Add to Player.
/// 2. Set startingHealth in Inspector (e.g., 3).
/// 3. Enemies should call TakeDamage(1) when they hit the Player.
/// </summary>
public class PlayerHealthSimple : MonoBehaviour
{
    public int startingHealth = 3;
    private int currentHealth;

    void Start()
    {
        currentHealth = startingHealth;
        UpdateUI();
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth < 0) currentHealth = 0;

        UpdateUI();

        if (currentHealth <= 0)
        {
            // Inform GameManager and disable player movement
            if (GameManagerSimple.Instance != null)
            {
                GameManagerSimple.Instance.Lose();
            }

            // Optional: disable movement component
            var move = GetComponent<PlayerMovementTopDownSimple>();
            if (move != null) move.enabled = false;
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > startingHealth)
            currentHealth = startingHealth;

        UpdateUI();
    }

    private void UpdateUI()
    {
        if (GameManagerSimple.Instance != null)
        {
            GameManagerSimple.Instance.SetHealth(currentHealth, startingHealth);
        }
    }
}
