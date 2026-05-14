using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Simple 2d player movement script.
//Left/Right A/D or Arrow Keys to move
//Jump when grounded, Space to jump.
//Add to the player. Assign GroundCheck and Ground Layer in the inspector.

[RequireComponent(typeof(Rigidbody2D))]

public class PlayerMovementSimple : MonoBehaviour
{
    //Declare the rigidbody2D
    private Rigidbody2D rb;

    //Movement variables
    [Header("Move")]
    public float moveSpeed = 7f;

    //Jump variables
    [Header("Jump")]
    public float jumpForce = 10f;


    public void BoostJump(float amount, float duration)
    {
        StartCoroutine(JumpBoostRoutine(amount, duration));
    }

    private IEnumerator JumpBoostRoutine(float amount, float duration)
    {
        jumpForce += amount;
        yield return new WaitForSeconds(duration);
        jumpForce -= amount;
    }


    //Ground Check variables
    [Header("Ground Check")]
    public Transform groundCheck; // tiny, empty shate at the feet of the player
    public float groundCheckRadius = 0.1f;
    public LayerMask groundLayer; // assign the Ground layer in the inspector

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        //Horizontal movement
        float inputX = Input.GetAxisRaw("Horizontal"); // -1, 0, or 1 based on A/D or Left/Right
        rb.velocity = new Vector2(inputX * moveSpeed, rb.velocity.y);

        // Jumping
        bool isGround = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        if (isGround && Input.GetButtonDown("Jump"))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }

    //Draw a small Ground Check Circle in Scene View
    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
