using UnityEngine;

/// <summary>
/// Damages the player when colliding with them.
/// Enemy needs a Collider (not trigger) and possibly a Rigidbody.
/// </summary>
public class EnemyDamagePlayer3D : MonoBehaviour
{
    public int damageAmount = 1;

    void OnCollisionEnter(Collision collision)
    {
        if (!collision.collider.CompareTag("Player")) return;

        PlayerHealthSimple3D player = collision.collider.GetComponent<PlayerHealthSimple3D>();
        if (player != null)
        {
            player.TakeDamage(damageAmount);
        }
    }
}
