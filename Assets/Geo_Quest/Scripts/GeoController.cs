using UnityEngine;
using UnityEngine.SceneManagement;

public class GeoController : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 7f;
    public float jumpForce = 11f;
    public float bounceForce = 30f;

    [Header("Camera")]
    public Transform cameraTransform;
    public float cameraOffsetX = 6f;

    [Header("Visual")]
    public Transform visualCube;
    public float rotationSpeed = 720f;

    [Header("Feel")]
    public float coyoteTime = 0.1f;

    [Header("Platforms")]
    public int normalPlatformLayer = 8;
    public int yellowPlatformLayer = 9;
    public float fallThroughTime = 0.2f;

    [Header("Water")]
    public string waterTag = "Water";
    public float waterSlowMultiplier = 0.5f;

    private Rigidbody2D rb;
    private Collider2D playerCollider;
    private SpriteRenderer sr;

    private bool isGrounded;
    private bool isInWater;
    private float targetRotation;
    private float coyoteCounter;
    private float baseSpeed;

    public bool isMoving = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();

        rb.gravityScale = 6.5f;

        if (visualCube == null)
            visualCube = transform;

        baseSpeed = speed;
    }

    void Update()
    {
        // Coyote time
        coyoteCounter -= Time.deltaTime;
        if (isGrounded) coyoteCounter = coyoteTime;

        // Jump
        if (Input.GetKeyDown(KeyCode.Space) && coyoteCounter > 0)
            Jump();

        // Fall through platforms
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            HandlePlatformFall(normalPlatformLayer);
            HandlePlatformFall(yellowPlatformLayer);
        }

        // Color switching 
        switch (true)
        {
            case bool _ when Input.GetKeyDown(KeyCode.Alpha1):
                sr.color = Color.red;
                break;
            case bool _ when Input.GetKeyDown(KeyCode.Alpha2):
                sr.color = Color.green;
                break;
            case bool _ when Input.GetKeyDown(KeyCode.Alpha3):
                sr.color = Color.blue;
                break;
        }

        // Rotation
        visualCube.rotation = Quaternion.RotateTowards(
            visualCube.rotation,
            Quaternion.Euler(0, 0, targetRotation),
            rotationSpeed * Time.deltaTime
        );
    }

    void FixedUpdate()
    {
        float currentSpeed = isInWater ? baseSpeed * waterSlowMultiplier : baseSpeed;

        if (isMoving)
            rb.velocity = new Vector2(currentSpeed, rb.velocity.y);
        else
            rb.velocity = new Vector2(Input.GetAxis("Horizontal") * speed, rb.velocity.y);

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

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;

            targetRotation = Mathf.Round(targetRotation / 90f) * 90f;
            visualCube.rotation = Quaternion.Euler(0, 0, targetRotation);
        }
        else
        {
            Die();
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = false;
    }

    //  SWITCH CASE FOR ALL TRIGGERS
    void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.tag)
        {
            case "Death":
                Die();
                break;

            case "Finish":
                LoadNextLevel();
                break;

            case "Bounce":
                rb.velocity = new Vector2(speed, bounceForce);
                isGrounded = false;
                targetRotation -= 90f;
                break;

            case "Coin":
                Destroy(collision.gameObject);
                break;

            default:
                if (collision.CompareTag(waterTag))
                    isInWater = true;
                break;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(waterTag))
            isInWater = false;
    }

    void Jump()
    {
        rb.velocity = new Vector2(speed, jumpForce);
        isGrounded = false;
        targetRotation -= 90f;

        HandlePlatformFall(normalPlatformLayer);
    }

    void Die()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void LoadNextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    void HandlePlatformFall(int layer)
    {
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(1 << layer);

        Collider2D[] results = new Collider2D[1];
        if (playerCollider.OverlapCollider(filter, results) > 0)
            StartCoroutine(FallThrough(results[0]));
    }

    System.Collections.IEnumerator FallThrough(Collider2D platform)
    {
        if (platform == null) yield break;

        Physics2D.IgnoreCollision(playerCollider, platform, true);
        yield return new WaitForSeconds(fallThroughTime);
        Physics2D.IgnoreCollision(playerCollider, platform, false);
    }
}