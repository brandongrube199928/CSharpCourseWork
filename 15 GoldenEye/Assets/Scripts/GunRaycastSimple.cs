using UnityEngine;

/// <summary>
/// Simple hitscan gun:
/// - Left mouse button to fire
/// - Raycast from camera forward
/// - Damages any EnemyHealthSimple3D it hits
/// </summary>
public class GunRaycastSimple : MonoBehaviour
{
    public float fireRate = 0.3f;    // time between shots
    public float maxDistance = 50f;  // ray length

    private float nextFireTime = 0f;

    void Update()
    {
        if (Input.GetButton("Fire1") && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;
            Fire();
        }
    }

    void Fire()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            // Try to damage an enemy
            EnemyHealthSimple3D enemy = hit.collider.GetComponent<EnemyHealthSimple3D>();
            if (enemy != null)
            {
                enemy.TakeDamage(1);
            }

            // Optional: visualize impact
            Debug.DrawLine(ray.origin, hit.point, Color.red, 0.2f);
        }
        else
        {
            // No hit, draw a short debug line
            Debug.DrawLine(ray.origin, ray.origin + ray.direction * maxDistance, Color.gray, 0.2f);
        }
    }
}
