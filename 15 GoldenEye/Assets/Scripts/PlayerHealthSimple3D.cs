using UnityEngine;

/// <summary>
/// Tracks player health and informs GameManager3D on death.
/// Setup:
/// 1. Add to Player.
/// 2. Set startingHealth in Inspector.
/// </summary>
public class PlayerHealthSimple3D : MonoBehaviour
{
    public int startingHealth = 5;
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
            // Tell GameManager the player lost
            if (GameManager3D.Instance != null)
            {
                GameManager3D.Instance.Lose();
            }

            // Disable movement
            var move = GetComponent<PlayerMovementFPS>();
            if (move != null) move.enabled = false;

            // Optional: unlock cursor
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
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
        if (GameManager3D.Instance != null)
        {
            GameManager3D.Instance.UpdateHealthUI(currentHealth, startingHealth);
        }
    }
}
