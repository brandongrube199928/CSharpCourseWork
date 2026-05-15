using UnityEngine;

/// <summary>
/// Simple enemy health for being shot.
/// Setup:
/// 1. Add to Enemy.
/// 2. Optionally set startingHealth.
/// 3. When health <= 0, destroy enemy and maybe check win condition.
/// </summary>
public class EnemyHealthSimple3D : MonoBehaviour
{
    public int startingHealth = 2;
    private int currentHealth;

    void Start()
    {
        currentHealth = startingHealth;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject);

        // Check if this was the last enemy
        if (GameObject.FindGameObjectsWithTag("Enemy").Length == 1)
        {
            // This enemy is still counted in the query until Destroy finishes,
            // but for quick prototype this is fine.
            if (GameManager3D.Instance != null)
            {
                GameManager3D.Instance.Win();
            }

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
