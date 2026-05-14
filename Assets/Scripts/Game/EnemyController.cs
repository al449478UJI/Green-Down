using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;// The speed at which the enemy will patrol when the player is not detected, can be set in the Inspector
    [SerializeField] private float patrolRadius = 2f;// The radius within which the enemy will patrol around its initial position, can be set in the Inspector
    [SerializeField] private float attackSpeed = 5f;// The speed at which the enemy will move towards the player, can be set in the Inspector
    [SerializeField] private Transform graphics;// Reference to the child Transform that contains the enemy's visual representation (sprite), used for flipping the sprite when changing direction
    private bool isLookingRight = true;// Flag to track the direction the enemy is facing, used for flipping the sprite when changing direction
    private bool isPatrolling = true;// Flag to track whether the enemy is currently patrolling, used to control behavior when the player is detected or lost
    private bool isChasing = false;// Flag to track whether the enemy is currently chasing the player, used to control behavior when the player is detected or lost
    private Vector3 graphicsOriginalScale;// Original scale of the enemy's graphics, used to reset the scale when flipping
    private Vector2 movement;// Vector to store the calculated movement direction towards the player
    private Vector2 centerPosition;// The initial position of the enemy, used as the center point for patrolling when the player is not detected
    private Vector2 patrolTarget;// The target position for patrolling when the player is not detected

    [Header("Attack")]
    [SerializeField] private Transform player;// The player's Transform that the enemy will target, can be set in the Inspector
    [SerializeField] private float detectionRange = 5f;// The range within which the enemy will detect the player, can be set in the Inspector
    [SerializeField] private int attackDamage = 1;// The amount of damage the enemy will inflict on the player when attacking, can be set in the Inspector
    [SerializeField] private float attackAnimationDuration = 0.5f;// Duration of the attack animation, can be set in the Inspector, used to control the timing of damage application and animations
    private Coroutine attackCoroutine;// Reference to the currently running attack coroutine, used to stop the coroutine when the player is lost or the enemy dies
    private bool attacking = false;// Flag to track whether the enemy is currently attacking, used to control attack behavior and animations
    private bool isPlayerDetected = false;// Flag to track whether the player has been detected
    private bool wasPlayerDetected = false;// Flag to track whether the player was detected in the previous frame, used to control behavior when the player is detected or lost

    [Header("Health")]
    [SerializeField] private int maxHealth = 20;// Maximum health for the enemy, can be set in the Inspector
    [SerializeField] private float cooldownAfterHit = 0.5f;// Cooldown time after the enemy takes damage before it can take damage again, can be set in the Inspector
    public bool damage = false;// Flag to track whether the enemy has recently taken damage and is in cooldown, used to prevent taking damage multiple times in quick succession and to control animations
    private int currentHealth;// Current health of the enemy, initialized in Start() to maxHealth
    private bool isDead = false;// Flag to track whether the enemy is dead, used to prevent multiple death triggers

    [Header("Utilities")]
    [SerializeField] private Animator animator;// Reference to the Animator component for controlling animations, can be set in the Inspector
    private Rigidbody2D rb;// Reference to the enemy's Rigidbody2D component for movement
    private BoxCollider2D boxCollider;// Reference to the enemy's BoxCollider2D component for collision detection

    private void Awake()
    {
        // Get the Rigidbody2D component attached to the enemy GameObject
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }

        if (boxCollider == null)
        {
            boxCollider = GetComponent<BoxCollider2D>();
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
        GetNewCenter();// Store the initial position of the enemy as the center point for patrolling

        patrolTarget = GetNewPatrolTarget();// Set the initial patrol target to a random point within the patrol radius

        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetBool("inchase", isChasing);// Update the "chasing" parameter in the Animator based on whether the enemy is currently chasing the player to control animations
        animator.SetBool("inpatrol", isPatrolling);// Update the "patrolling" parameter in the Animator based on whether the enemy is currently patrolling to control animations
        animator.SetBool("dead", isDead);// Update the "dead" parameter in the Animator based on whether the enemy is currently dead to control animations
        animator.SetBool("attack", attacking);// Update the "attack" parameter in the Animator based on whether the enemy is currently attacking to control animations
        animator.SetBool("damage", damage);// Update the "damage" parameter in the Animator based on whether the enemy has recently taken damage to control animations
    }

    private void FixedUpdate()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);// Calculate the distance from the enemy to the player
        bool detectedNow = distanceToPlayer <= detectionRange;// Check if the player is within the detection range and update the isPlayerDetected flag accordingly

        // Flip the enemy's sprite based on the direction of movement towards the player
        if (rb.linearVelocityX > 0 && !isLookingRight)
        {
            Flip(true); // Face right
        }
        else if (rb.linearVelocityX < 0 && isLookingRight)
        {
            Flip(false); // Face left
        }

        // If the player was detected in the previous frame but is no longer detected, get a new center position and patrol target for patrolling behavior
        if (wasPlayerDetected && !detectedNow)
        {
            GetNewCenter();
            patrolTarget = GetNewPatrolTarget();
        }

        // Update the detection flags for the current frame
        isPlayerDetected = detectedNow;
        wasPlayerDetected = detectedNow;

        // If the player is detected and is not invulnerable, chase the player; otherwise, continue patrolling
        if (isPlayerDetected && !PlayerHealth.instance.isInvulnerable && !PlayerHealth.instance.isDead && !damage)
        {
            isPatrolling = false;
            isChasing = true;
            ChasePlayer();
        }

        // If the player is not detected, continue patrolling
        else if (!damage && !attacking)
        {
            isPatrolling = true;
            isChasing = false;
            Patrol();
        }
    }

    // This method is called when the enemy collides with another collider (2D physics)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StartAttackAnimation();// Start the attack animation when colliding with the player, can be used to control the timing of damage application and animations
           

            Vector2 damageDirection = new Vector2(transform.position.x, 0);// Calculate the direction of the attack for applying knockback to the player, using the enemy's position on the x-axis and ignoring vertical direction for a horizontal knockback effect

            PlayerHealth.instance.TakeDamage(damageDirection, attackDamage);// Apply damage to the player using the TakeDamage method in the PlayerHealth script, passing the calculated damage direction and attack damage amount
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

    // Method to handle chasing behavior when the player is detected
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

    // Method to update the center position for patrolling, called when the player is lost after being detected
    private void GetNewCenter()
    {
        centerPosition = transform.position;// Store the initial position of the enemy as the center point for patrolling
    }

    // Method to flip the enemy's sprite based on the desired facing direction (true for right, false for left)
    private void Flip(bool faceRight)
    {
        isLookingRight = faceRight;// Update the isLookingRight flag based on the desired facing direction
        if (graphics != null)
        {
            Vector3 newScale = graphicsOriginalScale;
            newScale.x *= faceRight ? 1 : -1;// Flip the x scale of the graphics to face the correct direction
            graphics.localScale = newScale;// Apply the new scale to the graphics Transform
        }
    }

    // Method to apply damage to the enemy, reducing its current health and handling death if health reaches zero or below
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;// Reduce the enemy's current health by the specified damage amount

        // If the enemy is still alive after taking damage, stop chasing and patrolling, reset horizontal velocity, and start a cooldown before it can take damage again
        if (currentHealth > 0)
        {
            isChasing = false;
            isPatrolling = false;
            rb.linearVelocityX = 0;
            StartCoroutine(CooldownCoroutine());
        }

        // If the enemy's health reaches zero or below, call the Die method to handle death behavior
        else if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Method to handle the enemy's death behavior, disabling physics simulation and collisions, setting the isDead flag, and destroying the enemy GameObject after a delay
    private void Die()
    {
        // If the enemy is already marked as dead, return early to prevent multiple death triggers and ensure that the death behavior is only executed once
        if (isDead)
        {
            return;
        }

        rb.simulated = false;// Disable physics simulation for the enemy when it dies to prevent further movement or interactions
        boxCollider.enabled = false;// Disable the BoxCollider2D to prevent further collisions with the enemy when it dies
        isDead = true;// Set the isDead flag to true to prevent multiple death triggers
        Destroy(gameObject, 2f);// Destroy the enemy GameObject when its health reaches zero or below

        // Notify the ScoreManager that an enemy has been defeated to update the score and check for level completion, but only if the scoreManager reference is valid to avoid null reference errors
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.EnemyDefeated();// Call the EnemyDefeated method in the ScoreManager to update the score and check for level completion
        }
    }

    // Method to start the attack animation when colliding with the player, can be used to control the timing of damage application and animations
    private void StartAttackAnimation()
    {
        // If an attack coroutine is already running, stop it before starting a new one to prevent overlapping attack animations and ensure proper timing
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
        }

        attackCoroutine = StartCoroutine(AttackAnimationCoroutine());// Start the attack animation coroutine to handle the timing of the attack animation and damage application
    }

    // Coroutine to handle the timing of the attack animation, setting the attacking flag to true for the duration of the animation and then resetting it to false
    private IEnumerator AttackAnimationCoroutine()
    {
        attacking = true;// Set the attacking flag to true to trigger the attack animation
        yield return new WaitForSeconds(attackAnimationDuration);// Wait for the duration of the attack animation before allowing another attack
        attacking = false;// Set the attacking flag to false to end the attack animation
    }

    // Coroutine to handle the cooldown after the enemy takes damage, setting the damage flag to true for the duration of the cooldown and then resetting it to false
    private IEnumerator CooldownCoroutine()
    {
        damage = true;// Set the damage flag to true to indicate that the enemy has recently taken damage and is in cooldown
        yield return new WaitForSeconds(cooldownAfterHit);// Wait for the duration of the cooldown before allowing the enemy to take damage again
        damage = false;// Set the damage flag to false to indicate that the enemy can take damage again after the cooldown
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
