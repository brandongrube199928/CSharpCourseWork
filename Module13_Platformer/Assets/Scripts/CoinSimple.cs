using UnityEngine;

/// <summary>
/// Attach to a coin object with a Collider2D set to "Is Trigger".
/// On Player trigger, adds to score and destroys the coin.
/// </summary>
public class CoinSimple : MonoBehaviour
{
    public int value = 1;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (GameManagerSimple.Instance != null)
        {
            GameManagerSimple.Instance.AddScore(value);
        }
        Destroy(gameObject);
    }
}
