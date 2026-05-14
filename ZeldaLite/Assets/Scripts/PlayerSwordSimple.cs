using UnityEngine;

/// <summary>
/// Handles the sword attack:
/// - When Space is pressed, briefly enables the sword hitbox.
/// </summary>
public class PlayerSwordSimple : MonoBehaviour
{
    public GameObject swordHitbox;
    public float attackDuration = 0.15f;
    private bool isAttacking = false;
    private float attackTimer = 0f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isAttacking)
        {
            StartAttack();
        }

        if (isAttacking)
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0f)
            {
                EndAttack();
            }
        }
    }

    void StartAttack()
    {
        isAttacking = true;
        attackTimer = attackDuration;
        if (swordHitbox != null) swordHitbox.SetActive(true);
    }

    void EndAttack()
    {
        isAttacking = false;
        if (swordHitbox != null) swordHitbox.SetActive(false);
    }
}
