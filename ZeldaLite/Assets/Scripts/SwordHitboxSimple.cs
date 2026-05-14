using UnityEngine;

/// <summary>
/// Attach to SwordHitbox (child of Player).
/// When active, it destroys enemies it touches.
/// Enemy objects should have tag "Enemy".
/// </summary>
public class SwordHitboxSimple : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Enemy")) return;

        // Destroy enemy and add score (optional)
        Destroy(other.gameObject);

        if (GameManagerSimple.Instance != null)
        {
            GameManagerSimple.Instance.AddScore(1);
        }

        // Optional: check if any enemies remain, then auto-win
        if (GameObject.FindGameObjectsWithTag("Enemy").Length == 1)
        {
            // this was the last enemy (we haven't been destroyed yet)
            GameManagerSimple.Instance?.Win();
        }
    }
}
