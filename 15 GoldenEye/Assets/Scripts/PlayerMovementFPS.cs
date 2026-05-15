using UnityEngine;

/// <summary>
/// Very simple first-person controller:
/// - Move with WASD
/// - Look around with mouse
/// Setup:
/// 1. Add this to Player (with Rigidbody + CapsuleCollider).
/// 2. Drag PlayerCamera transform into cameraTransform in Inspector.
/// 3. Lock/Hide cursor when playing (optional but nice).
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class PlayerMovementFPS : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Mouse Look")]
    public Transform cameraTransform;
    public float mouseSensitivity = 2f;
    public float minPitch = -80f;
    public float maxPitch = 80f;

    private Rigidbody rb;
    private float pitch = 0f;
    private Vector2 inputMove;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // extra safety
    }

    void Start()
    {
        // Optional: lock cursor to center
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // --- Movement input ---
        float x = Input.GetAxisRaw("Horizontal");  // A/D or Left/Right
        float z = Input.GetAxisRaw("Vertical");    // W/S or Up/Down
        inputMove = new Vector2(x, z).normalized;

        // --- Mouse look ---
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Yaw (left-right) on player body
        transform.Rotate(Vector3.up * mouseX);

        // Pitch (up-down) on camera
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        if (cameraTransform != null)
        {
            cameraTransform.localEulerAngles = new Vector3(pitch, 0f, 0f);
        }
    }

    void FixedUpdate()
    {
        // Move relative to where the player is facing
        Vector3 move = transform.right * inputMove.x + transform.forward * inputMove.y;
        Vector3 targetPosition = rb.position + move * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(targetPosition);
    }
}
