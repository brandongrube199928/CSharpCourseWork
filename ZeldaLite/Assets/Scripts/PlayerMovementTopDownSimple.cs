using UnityEngine;

/// <summary>
/// Simple top-down movement:
/// - Uses Rigidbody2D
/// - Moves with WASD or Arrow keys
/// Setup:
/// 1. Add to Player object.
/// 2. Player must have Rigidbody2D (Gravity Scale = 0, Freeze Rotation Z).
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovementTopDownSimple : MonoBehaviour
{
    public float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Vector2 input;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Get input: horizontal (A/D or arrow keys) and vertical (W/S or arrow keys)
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        input = new Vector2(x, y).normalized;
    }

    void FixedUpdate()
    {
        // Apply movement in physics step
        rb.velocity = input * moveSpeed;
    }
}
