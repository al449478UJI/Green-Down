using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Horizontal Movement")]
    [SerializeField] private float moveSpeed = 5f;// Serialized fields allow you to set these values in the Unity Inspector
    [SerializeField] private Transform graphics;// Transform for the player's graphics, used to flip the sprite based on movement direction
    private Vector2 movement;// Vector2 to store the movement input from the player
    public bool isFacingRight = true;// bool to track the direction the player is facing, used for flipping the sprite
    private Vector3 graphicsOriginalScale;// Original scale of the graphics, used to reset the scale when flipping

    [Header("Utility")]
    [SerializeField] private Animator animator;// Animator component for controlling animations, can be set in the Inspector
    private Rigidbody2D rb;// Rigidbody2D component for physics-based movement

    [Header("Jump")]
    [SerializeField] private float jumpForce = 5f;// Jump force for the player, can be set in the Inspector
    [SerializeField] private Transform groundCheck;// Transform used to check if the player is grounded, can be set in the Inspector
    [SerializeField] private float groundCheckRadius = 0.2f;// Radius for the ground check, can be set in the Inspector
    [SerializeField] private LayerMask groundLayer;// LayerMask to specify which layers are considered ground for the ground check, can be set in the Inspector
    private bool isGrounded;// bool to track if the player is currently grounded, used for allowing jumps only when grounded
    private bool isJumping;// bool to track if the player is currently jumping, used for controlling jump animations and logic


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
        float horizontalSpeed = Mathf.Abs(rb.linearVelocity.x);// Get the absolute value of the player's horizontal velocity to determine the speed for animation purposes
        float verticalSpeed = rb.linearVelocity.y;// Get the player's vertical velocity for potential use in animations (not currently used in this code)

        animator.SetFloat("movement", horizontalSpeed);// Update the "Speed" parameter in the Animator based on the player's horizontal velocity to control animations
        animator.SetBool("grounded", isGrounded);// Update the "isGrounded" parameter in the Animator based on whether the player is currently grounded to control animations
        animator.SetBool("jump", isJumping);// Update the "isJumping" parameter in the Animator based on whether the player is currently jumping to control animations
        animator.SetFloat("jumpspeed", verticalSpeed);// Update the "jumpspeed" parameter in the Animator based on the player's vertical velocity for potential use in animations (not currently used in this code)
    }

    // FixedUpdate is called at a fixed interval and is used for physics updates
    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(movement.x * moveSpeed, rb.linearVelocity.y);// Move the player based on the input received

        // Flip the sprite based on the direction of movement
        if (movement.x > 0 && !isFacingRight)
        {
            flip(true); // Face right
        }
        else if (movement.x < 0 && isFacingRight)
        {
            flip(false); // Face left
        }


        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);// Check if the player is grounded by using Physics2D.OverlapCircle to check for colliders in the ground layer

        // Jump testing: Check if the player is currently jumping and if the vertical velocity is less than or equal to 0 (indicating the player is falling or has reached the peak of the jump)
        if (!isJumping && rb.linearVelocity.y <= 0)
        {

        }

        // Jump testing: Check if the player is currently jumping, is grounded, and has a vertical velocity of 0 (indicating the player has landed)
        if (isJumping && isGrounded && rb.linearVelocity.y == 0)
        {
            isJumping = false;
        }
    }

    // This method is called by the Input System when the player provides movement input
    public void OnMove(InputValue value)
    {
        movement = value.Get<Vector2>();// Get the movement input as a Vector2 and store it in the movement variable
    }

    // This method is called by the Input System when the player presses the jump button
    public void OnJump(InputValue button)
    {
        // Check if the jump button is pressed and if the player is grounded before allowing the jump
        if (button.isPressed && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);// Reset vertical velocity before applying jump force

            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);// Apply a vertical force to the Rigidbody2D to make the player jump

            // Set jump testing variables
            isJumping = true;
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
