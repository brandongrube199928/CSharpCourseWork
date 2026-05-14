using UnityEngine;

/// <summary>
/// Attach to spikes or a death zone (non-trigger collider).
/// On collision with Player, calls Lose() and prompts restart.
/// </summary>
public class HazardSimple : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D col)
    {
        if (!col.collider.CompareTag("Player")) return;
        if (GameManagerSimple.Instance != null)
        {
            GameManagerSimple.Instance.Lose();
        }
    }
}
