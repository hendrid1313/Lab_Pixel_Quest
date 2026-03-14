using UnityEngine;
using UnityEngine.SceneManagement;

public class GeoController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 7f;            // constant rightward speed
    public float jumpForce = 11f;       // normal jump height
    public float bounceForce = 30f;     // bounce pad jump height
    public string nextlevel = "Scene2"; // assign your next scene in Inspector

    [Header("Camera Settings")]
    public Transform cameraTransform;
    public float cameraOffsetX = 6f;

    [Header("Visual Settings")]
    public Transform visualCube;        // child sprite for rotation
    public float rotationSpeed = 720f;  // degrees per second

    [Header("Jump Feel Settings")]
    public float coyoteTime = 0.1f;     // small grace period after leaving ground

    [Header("Platform Settings")]
    public int normalPlatformLayer = 8;     // your normal one-way platform layer
    public int yellowPlatformLayer = 9;     // yellow platforms that auto-fall
    public float fallThroughTime = 0.2f;    // duration to ignore collision

    [Header("Water Settings")]
    public string waterTag = "Water";       // tag for water triggers
    public float waterSlowMultiplier = 0.5f; // multiplier for speed in water

    private Rigidbody2D rb;
    private Collider2D playerCollider;
    private bool isGrounded;
    private float targetRotation = 0f;
    private float coyoteCounter;
    private float originalSpeed;
    private bool isInWater = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();
        rb.gravityScale = 6.5f; // tuned for GD feel

        if (visualCube == null)
            visualCube = transform;

        originalSpeed = speed;
    }

    void Update()
    {
        // Update coyote counter
        coyoteCounter -= Time.deltaTime;
        if (isGrounded)
            coyoteCounter = coyoteTime;

        // Jump input
        if (Input.GetKeyDown(KeyCode.Space) && coyoteCounter > 0)
        {
            Jump(jumpForce);
        }

        // Fall-through when pressing down
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            HandlePlatformFall(normalPlatformLayer);
            HandlePlatformFall(yellowPlatformLayer);
        }

        // Rotate visual cube smoothly
        visualCube.rotation = Quaternion.RotateTowards(
            visualCube.rotation,
            Quaternion.Euler(0, 0, targetRotation),
            rotationSpeed * Time.deltaTime
        );
    }

    void FixedUpdate()
    {
        // Adjust speed in water
        float currentSpeed = isInWater ? originalSpeed * waterSlowMultiplier : originalSpeed;

        // Move player forward
        rb.velocity = new Vector2(currentSpeed, rb.velocity.y);

        // Fall through normal platforms when falling
        if (rb.velocity.y < 0)
            HandlePlatformFall(normalPlatformLayer);

        // Auto fall through yellow platforms if on top
        if (isGrounded)
            HandlePlatformFall(yellowPlatformLayer);
    }

    void LateUpdate()
    {
        if (cameraTransform != null)
            cameraTransform.position = new Vector3(transform.position.x + cameraOffsetX, 0, -10);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;

            // Snap rotation to nearest 90°
            targetRotation = Mathf.Round(targetRotation / 90f) * 90f;
            visualCube.rotation = Quaternion.Euler(0, 0, targetRotation);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Death"))
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        else if (collision.CompareTag("Finish"))
            SceneManager.LoadScene(nextlevel);
        else if (collision.CompareTag("Bounce"))
        {
            rb.velocity = new Vector2(speed, bounceForce);
            isGrounded = false;
            targetRotation -= 90f;
        }
        else if (collision.CompareTag(waterTag))
            isInWater = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(waterTag))
            isInWater = false;
    }

    private void Jump(float force)
    {
        rb.velocity = new Vector2(speed, force);
        isGrounded = false;
        targetRotation -= 90f;

        // Fall through normal platforms on jump
        HandlePlatformFall(normalPlatformLayer);
    }

    // ---------------- GD-style fall-through logic ----------------
    private void HandlePlatformFall(int layer)
    {
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(1 << layer);
        Collider2D[] results = new Collider2D[1];

        int count = playerCollider.OverlapCollider(filter, results);
        if (count > 0)
        {
            StartCoroutine(FallThroughCollider(results[0]));
        }
    }

    private System.Collections.IEnumerator FallThroughCollider(Collider2D platform)
    {
        if (platform == null) yield break;

        Physics2D.IgnoreCollision(playerCollider, platform, true);
        yield return new WaitForSeconds(fallThroughTime);
        Physics2D.IgnoreCollision(playerCollider, platform, false);
    }
}
