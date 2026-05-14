using UnityEngine;

/// <summary>
/// Very simple enemy patrol:
/// - Moves between two points A and B.
/// Setup:
/// 1. Create Enemy object with SpriteRenderer + Collider2D + Rigidbody2D (Dynamic or Kinematic).
/// 2. Create two empty child objects as waypoints: PointA and PointB.
/// 3. Assign them in the Inspector.
/// 4. Tag Player as "Player".
/// </summary>
public class EnemyPatrolSimple : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public float moveSpeed = 2f;

    private Transform target;

    void Start()
    {
        target = pointB;
    }

    void Update()
    {
        if (target == null) return;

        transform.position = Vector2.MoveTowards(
            transform.position,
            target.position,
            moveSpeed * Time.deltaTime
        );

        float dist = Vector2.Distance(transform.position, target.position);
        if (dist < 0.05f)
        {
            // Switch target
            target = (target == pointA) ? pointB : pointA;
        }
    }
}
