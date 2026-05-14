using UnityEngine;

/// <summary>
/// Attach to a goal object with a Collider2D set to "Is Trigger".
/// When Player enters, show Win message.
/// </summary>
public class GoalSimple : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (GameManagerSimple.Instance != null)
        {
            GameManagerSimple.Instance.Win();
        }
    }
}
