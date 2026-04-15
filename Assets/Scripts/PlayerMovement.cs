using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    // Serialized fields allow you to set these values in the Unity Inspector
    [SerializeField] private float moveSpeed = 5f;

    // Vector2 to store the movement input from the player
    private Vector2 movement;

    // Jump force for the player, can be set in the Inspector
    [SerializeField] private float jumpForce = 5f;

    // Rigidbody2D component for physics-based movement
    private Rigidbody2D rb;

    // Transform for the player's graphics, used to flip the sprite based on movement direction
    [SerializeField] private Transform graphics;

    // Original scale of the graphics, used to reset the scale when flipping
    private Vector3 graphicsOriginalScale;

    // bool to track the direction the player is facing, used for flipping the sprite
    private bool isFacingRight = true;

    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        // Get the Rigidbody2D component attached to the player GameObject
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }

        // Store the original scale of the graphics Transform for later use when flipping
        if (graphics != null)
        {
            graphicsOriginalScale = graphics.localScale;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    // FixedUpdate is called at a fixed interval and is used for physics updates
    private void FixedUpdate()
    {
        // Move the player based on the input received
        rb.linearVelocity = new Vector2(movement.x * moveSpeed, rb.linearVelocity.y);

        // Flip the sprite based on the direction of movement
        if (movement.x > 0 && !isFacingRight)
        {
            flip(true); // Face right
        }
        else if (movement.x < 0 && isFacingRight)
        {
            flip(false); // Face left
        }
    }

    // This method is called by the Input System when the player provides movement input
    public void OnMove(InputValue value)
    {
        // Get the movement input as a Vector2 and store it in the movement variable
        movement = value.Get<Vector2>();
    }

    // This method is called by the Input System when the player presses the jump button
    public void OnJump(InputValue button)
    {
        // Check if the jump button is pressed
        if (button.isPressed)
        {
            // Reset vertical velocity before applying jump force
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);

            // Apply a vertical force to the Rigidbody2D to make the player jump
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    private void flip(bool faceRight)
    {

        isFacingRight = faceRight;

        // Check if the graphics Transform is assigned before trying to flip it
        if (graphics == null)
        {
            return;
        }

        // Flip the graphics by changing the local scale's x value
        graphics.localScale = new Vector3(
            Mathf.Abs(graphicsOriginalScale.x) * (faceRight ? 1f : -1f),
            graphicsOriginalScale.y,
            graphicsOriginalScale.z
        );
    }
}
