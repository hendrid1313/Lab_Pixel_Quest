using UnityEngine;

public class ShipController : MonoBehaviour
{
    public float forwardSpeed = 6f;
    public float controlSpeed = 4f;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector2 movement = new Vector2(forwardSpeed + horizontal * controlSpeed, vertical * controlSpeed);

        rb.velocity = movement;
    }
}