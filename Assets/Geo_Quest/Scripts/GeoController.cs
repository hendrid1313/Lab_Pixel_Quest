using UnityEngine;
using UnityEngine.SceneManagement;

public class GeoController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 7f;
    public float jumpForce = 11f;
    public float bounceForce = 30f;

    [Header("Camera Settings")]
    public Transform cameraTransform;
    public float cameraOffsetX = 6f;

    [Header("Visual Settings")]
    public Transform visualCube;
    public float rotationSpeed = 720f;

    [Header("Jump Feel Settings")]
    public float coyoteTime = 0.1f;

    [Header("Platform Settings")]
    public int normalPlatformLayer = 8;
    public int yellowPlatformLayer = 9;
    public float fallThroughTime = 0.2f;

    [Header("Water Settings")]
    public string waterTag = "Water";
    public float waterSlowMultiplier = 0.5f;

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
        rb.gravityScale = 6.5f;

        if (visualCube == null)
            visualCube = transform;

        originalSpeed = speed;
    }

    void Update()
    {
        coyoteCounter -= Time.deltaTime;

        if (isGrounded)
            coyoteCounter = coyoteTime;

        if (Input.GetKeyDown(KeyCode.Space) && coyoteCounter > 0)
        {
            Jump(jumpForce);
        }

        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            HandlePlatformFall(normalPlatformLayer);
            HandlePlatformFall(yellowPlatformLayer);
        }

        visualCube.rotation = Quaternion.RotateTowards(
            visualCube.rotation,
            Quaternion.Euler(0, 0, targetRotation),
            rotationSpeed * Time.deltaTime
        );
    }

    void FixedUpdate()
    {
        float currentSpeed = isInWater ? originalSpeed * waterSlowMultiplier : originalSpeed;
        rb.velocity = new Vector2(currentSpeed, rb.velocity.y);

        if (rb.velocity.y < 0)
            HandlePlatformFall(normalPlatformLayer);

        if (isGrounded)
            HandlePlatformFall(yellowPlatformLayer);
    }

    void LateUpdate()
    {
        if (cameraTransform != null)
            cameraTransform.position = new Vector3(transform.position.x + cameraOffsetX, 0, -10);
    }

    // ✅ COLLISION (SOLID OBJECTS)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Ground logic
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;

            targetRotation = Mathf.Round(targetRotation / 90f) * 90f;
            visualCube.rotation = Quaternion.Euler(0, 0, targetRotation);
        }
        else
        {
            // 💀 ANYTHING NOT GROUND = DEATH
            Die();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = false;
    }

    // ✅ TRIGGERS (optional special objects)
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Death"))
        {
            Die();
        }
        else if (collision.CompareTag("Finish"))
        {
            LoadNextLevel();
        }
        else if (collision.CompareTag("Bounce"))
        {
            rb.velocity = new Vector2(speed, bounceForce);
            isGrounded = false;
            targetRotation -= 90f;
        }
        else if (collision.CompareTag(waterTag))
        {
            isInWater = true;
        }
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

        HandlePlatformFall(normalPlatformLayer);
    }

    private void Die()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void LoadNextLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex + 1);
    }

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