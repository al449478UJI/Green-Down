using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;// The speed at which the enemy will patrol when the player is not detected, can be set in the Inspector
    [SerializeField] private float patrolRadius = 2f;// The radius within which the enemy will patrol around its initial position, can be set in the Inspector
    [SerializeField] private float attackSpeed = 5f;// The speed at which the enemy will move towards the player, can be set in the Inspector
    private Vector2 movement;// Vector to store the calculated movement direction towards the player
    private Vector2 centerPosition;// The initial position of the enemy, used as the center point for patrolling when the player is not detected
    private Vector2 patrolTarget;// The target position for patrolling when the player is not detected

    [Header("Attack")]
    [SerializeField] private Transform player;// The player's Transform that the enemy will target, can be set in the Inspector
    [SerializeField] private float detectionRange = 5f;// The range within which the enemy will detect the player, can be set in the Inspector
    [SerializeField] private int attackDamage = 1;// The amount of damage the enemy will inflict on the player when attacking, can be set in the Inspector
    private bool isPlayerDetected = false;// Flag to track whether the player has been detected
    private bool wasPlayerDetected = false;// Flag to track whether the player was detected in the previous frame, used to control behavior when the player is detected or lost

    [Header("Health")]
    [SerializeField] private int maxHealth = 20;// Maximum health for the enemy, can be set in the Inspector
    private int currentHealth;// Current health of the enemy, initialized in Start() to maxHealth
    private bool isDead = false;// Flag to track whether the enemy is dead, used to prevent multiple death triggers

    [Header("Utilities")]
    [SerializeField] private PlayerHealth playerHealth;// Reference to the player's health component for applying damage
    private Rigidbody2D rb;// Reference to the enemy's Rigidbody2D component for movement

    private void Awake()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetNewCenter();// Store the initial position of the enemy as the center point for patrolling

        patrolTarget = GetNewPatrolTarget();// Set the initial patrol target to a random point within the patrol radius

        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);// Calculate the distance from the enemy to the player
        bool detectedNow = distanceToPlayer <= detectionRange;// Check if the player is within the detection range and update the isPlayerDetected flag accordingly

        // If the player was detected in the previous frame but is no longer detected, get a new center position and patrol target for patrolling behavior
        if (wasPlayerDetected && !detectedNow)
        {
            GetNewCenter();
            patrolTarget = GetNewPatrolTarget();
        }

        // Update the detection flags for the current frame
        isPlayerDetected = detectedNow;
        wasPlayerDetected = detectedNow;

        if (isPlayerDetected && !playerHealth.isInvulnerable)
        {
            ChasePlayer();
        }

        else
        {
            Patrol();
        }
    }

    // This method is called when the enemy collides with another collider (2D physics)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerHealth = collision.gameObject.GetComponent<PlayerHealth>();

            Vector2 damageDirection = new Vector2(transform.position.x, 0);

            playerHealth.TakeDamage(damageDirection, attackDamage);
        }
    }

    // Method to handle patrolling behavior when the player is not detected
    private void Patrol()
    {
        float distanceToTarget = Mathf.Abs(transform.position.x - patrolTarget.x);// Calculate the distance from the enemy to the current patrol target

        // If the enemy is close enough to the patrol target, get a new patrol target
        if (distanceToTarget < 0.1f)
        {
            patrolTarget = GetNewPatrolTarget();
        }

        float directionX = Mathf.Sign(patrolTarget.x - transform.position.x);// Calculate the horizontal direction towards the patrol target (1 for right, -1 for left)

        rb.linearVelocity = new Vector2(directionX * moveSpeed, rb.linearVelocity.y);// Move towards the patrol target at the specified move speed
    }

    private void ChasePlayer()
    {
        // Calculate movement direction towards the player
        movement = (player.position - transform.position).normalized * attackSpeed;
        rb.linearVelocityX = movement.x;// Move towards the player at the specified attack speed
    }

    // Method to get a new random patrol target within the patrol radius around the center position
    private Vector2 GetNewPatrolTarget()
    {
        float randomX = Random.Range(-patrolRadius, patrolRadius);

        return new Vector2(centerPosition.x + randomX, centerPosition.y);
    }

    private void GetNewCenter()
    {
        centerPosition = transform.position;// Store the initial position of the enemy as the center point for patrolling
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;// Reduce the enemy's current health by the specified damage amount
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead)
        {
            return;
        }
        isDead = true;// Set the isDead flag to true to prevent multiple death triggers
        Destroy(gameObject);// Destroy the enemy GameObject when its health reaches zero or below
    }

    // This method is called by Unity to draw Gizmos in the editor, used here to visualize the patrol area and detection range of the enemy
    private void OnDrawGizmosSelected()
    {
        Vector2 center;

        if (Application.isPlaying)
        {
            center = centerPosition;
        }
        else
        {
            center = transform.position;
        }

        Vector2 leftPoint = new Vector2(center.x - patrolRadius, center.y);
        Vector2 rightPoint = new Vector2(center.x + patrolRadius, center.y);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(leftPoint, rightPoint);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
