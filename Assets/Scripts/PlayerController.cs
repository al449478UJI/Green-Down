using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // Serialized fields allow you to set these values in the Unity Inspector
    [SerializeField] private float moveSpeed = 5f;
    // Vector2 to store the movement input from the player
    private Vector2 movement;
    // Rigidbody2D component for physics-based movement
    private Rigidbody2D rb;
    // SpriteRenderer component to handle the player's sprite
    [SerializeField] private SpriteRenderer sprite;

    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        // Move the player based on the input received
        rb.linearVelocity = movement * moveSpeed;
        // Flip the sprite based on the direction of movement
        if (movement.x > 0 && sprite.flipX)
        {
            sprite.flipX = false; // Face right
        }
        else if (movement.x < 0 && !sprite.flipX)
        {
            sprite.flipX = true; // Face left
        }
    }

    // This method is called by the Input System when the player provides movement input
    public void OnMove(InputValue value)
    {
        movement = value.Get<Vector2>();
    }
}
