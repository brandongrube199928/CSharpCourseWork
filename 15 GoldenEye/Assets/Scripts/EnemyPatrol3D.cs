using UnityEngine;
// Made by Gemini; full credit to the AI!
// Brandon and I made this game together, and we both contributed to the code.
// Brandon came up with the idea for the basic structure, and I helped him implement it in Unity.


// This script controls the enemy's behavior in a 3D environment, allowing it to patrol between two points and then chase the player after completing a full lap.
[RequireComponent(typeof(Rigidbody))]
public class EnemyPatrol3D : MonoBehaviour
{
    // State machine for enemy behavior
    private enum EnemyState { Patrolling, SeekingPlayer }
    private EnemyState currentState = EnemyState.Patrolling;

    [Header("Patrol Settings")]
    // These should be assigned in the Unity Inspector to the empty GameObjects representing the patrol points
    public Transform pointA;
    public Transform pointB;
    public float moveSpeed = 2f;

    [Header("Chase Settings")]
    public float chaseSpeed = 3.5f;

    // Internal variables
    private Transform targetPoint;
    private Transform playerTransform;
    private Rigidbody rb;
    private int patrolPointsVisited = 0;

    // Awake is called when the script instance is being loaded
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
        targetPoint = pointB; // Start path towards B

        // Find player transform dynamically using your GameManager3D instance
        if (GameManager3D.Instance != null)
        {
            // Assumes GameManager3D script is attached to your player or has a reference
            // If GameManager is on a separate object, find player by tag instead:
            playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        }
    }

    // FixedUpdate is called at a fixed interval and is independent of frame rate. Put physics code here.
    void FixedUpdate()
    {
        // Safety check for missing setup
        if (pointA == null || pointB == null) return;

        Vector3 targetPosition = Vector3.zero;
        float currentSpeed = moveSpeed;

        // --- STATE MACHINE LOGIC ---
        if (currentState == EnemyState.Patrolling)
        {
            targetPosition = targetPoint.position;
            currentSpeed = moveSpeed;

            // Check if we arrived at the current patrol destination
            if (Vector3.Distance(rb.position, targetPosition) < 0.2f)
            {
                patrolPointsVisited++;

                // Changed to 2 so the enemy attacks right after finishing its first full loop
                if (patrolPointsVisited >= 2)
                {
                    currentState = EnemyState.SeekingPlayer;

                    // Send the jump-scare alert to your GameManager's UI text elements
                    if (GameManager3D.Instance != null)
                    {
                        GameManager3D.Instance.ShowMessage("YOUR ENEMIES ARE HUNTING YOU!");

                        // Automatically call ClearAlertText after 3 seconds to keep UI clean
                        Invoke(nameof(ClearAlertText), 3f);
                    }
                }
                else
                {
                    // Alternate patrol targets
                    targetPoint = (targetPoint == pointA) ? pointB : pointA;
                }
            }
        }
        else if (currentState == EnemyState.SeekingPlayer)
        {
            // Fallback tracking check if player wasn't found at Start
            if (playerTransform == null)
            {
                playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
                if (playerTransform == null) return; // Still missing
            }

            targetPosition = playerTransform.position;
            currentSpeed = chaseSpeed;
        }

        // --- PHYSICAL MOVEMENT & ROTATION ---
        // Move towards chosen destination smoothly
        Vector3 nextPosition = Vector3.MoveTowards(rb.position, targetPosition, currentSpeed * Time.fixedDeltaTime);
        rb.MovePosition(nextPosition);

        // Turn to look where it is going
        Vector3 direction = targetPosition - rb.position;
        direction.y = 0f; // Keep upright
        if (direction.magnitude > 0.01f)
        {
            rb.rotation = Quaternion.LookRotation(direction);
        }
    }

    // Clears the alert text if the game is still active
    private void ClearAlertText()
    {
        if (GameManager3D.Instance != null)
        {
            GameManager3D.Instance.ShowMessage("");
        }
    }
}
