using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GeoController : MonoBehaviour
{
    private Rigidbody2D rb;
    public int speed = 5;
    public string nextlevel = "Scene";

    // SpriteRenderer reference for color change
    private SpriteRenderer spriteRenderer;

    // Colors to switch between
    public Color color1 = Color.red;
    public Color color2 = Color.green;
    public Color color3 = Color.blue;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
        {
            Debug.LogError("No SpriteRenderer found on this GameObject!");
        }
    }

    void Update()
    {
        // --- Movement ---
        float xInput = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(xInput * speed, rb.velocity.y);

        // --- Color change ---
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            spriteRenderer.color = color1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            spriteRenderer.color = color2;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            spriteRenderer.color = color3;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Death"))
        {
            string thislevel = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(thislevel);
        }
        else if (collision.CompareTag("Finish"))
        {
            SceneManager.LoadScene(nextlevel);
        }
    }
} // <-- FINAL closing brace for the class