using UnityEngine;

/// <summary>
/// Damages the player when colliding with them.
/// Enemy needs a Collider2D (non-trigger) and optionally Rigidbody2D.
/// </summary>
public class EnemyDamageSimple : MonoBehaviour
{
    public int damageAmount = 1;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.collider.CompareTag("Player")) return;

        PlayerHealthSimple playerHealth = collision.collider.GetComponent<PlayerHealthSimple>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damageAmount);
        }
    }
}
