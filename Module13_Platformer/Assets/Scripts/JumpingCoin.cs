using UnityEngine;

public class JumpBoostCoin : MonoBehaviour
{
    public float boostAmount = 5f;
    public float boostDuration = 3f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerMovementSimple player = other.GetComponent<PlayerMovementSimple>();
        if (player != null)
        {
            player.BoostJump(boostAmount, boostDuration);
        }

        Destroy(gameObject);
    }
}
