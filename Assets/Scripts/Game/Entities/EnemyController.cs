using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;// The speed at which the enemy will patrol when the player is not detected, can be set in the Inspector
    [SerializeField] private float patrolRadius = 2f;// The radius within which the enemy will patrol around its initial position, can be set in the Inspector
    [SerializeField] private float attackSpeed = 5f;// The speed at which the enemy will move towards the player, can be set in the Inspector
    [SerializeField] private Transform graphics;// Reference to the child Transform that contains the enemy's visual representation (sprite), used for flipping the sprite when changing direction

    private bool isLookingRight = true;// Flag to track the direction the enemy is facing, used to flip the sprite when changing direction
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

    [Header("Sight and Detection")]
    [SerializeField] private LayerMask obstacleLayer;// LayerMask to specify which layers are considered obstacles for line of sight checks, can be set in the Inspector
    [SerializeField] private Transform sightOrigin;// Transform representing the origin point for line of sight checks, can be set in the Inspector
    [SerializeField] private float wallCheckDistance = 0.5f;// Distance for checking if there are walls or obstacles between the enemy and the player, can be set in the Inspector
    [SerializeField] private int maxPatrolTargetAttempts = 10;// Maximum number of attempts to find a valid patrol target that is not blocked by obstacles, can be set in the Inspector
    [SerializeField] private float stuckDuration = 1f;// Duration to consider the enemy stuck if it cannot move towards the player, can be set in the Inspector
    [SerializeField] private float minimumMovementPerFixedUpdate = 0.1f;// Minimum distance the enemy must move towards the player in each FixedUpdate to avoid being considered stuck, can be set in the Inspector
    [SerializeField] private float colliderCheckDistance = 0.08f;// Distance for checking if there are colliders between the enemy and the player, used in line of sight checks, can be set in the Inspector

    private ContactFilter2D obstacleContactFilter;// ContactFilter2D used for checking obstacles in line of sight and patrol path, initialized in Start() to use the specified obstacle layer
    private RaycastHit2D[] obstacleHits = new RaycastHit2D[4];// Array to store the results of raycasts for checking obstacles in line of sight and patrol path, used in line of sight checks and patrol target validation
    private Collider2D playerCollider;// Reference to the player's Collider2D component for line of sight checks
    private float stuckTimer = 0f;// Timer to track how long the enemy has been stuck when trying to move towards the player, used to trigger getting a new patrol target if the enemy is stuck
    private Vector2 lastPosition;// Variable to store the enemy's position in the last FixedUpdate for calculating movement towards the player and detecting if the enemy is stuck

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

        obstacleContactFilter = new ContactFilter2D();// Initialize the ContactFilter2D for checking obstacles in line of sight and patrol path

        obstacleContactFilter.SetLayerMask(obstacleLayer);// Set the layer mask of the ContactFilter2D to the specified obstacle layer to ensure that only obstacles are detected in line of sight checks and patrol target validation

        obstacleContactFilter.useTriggers = false;// Set the ContactFilter2D to not detect trigger colliders, as we only want to consider solid obstacles for line of sight checks and patrol target validation
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetNewCenter();// Store the initial position of the enemy as the center point for patrolling

        // Get the player's Collider2D component for line of sight checks, but only if the player reference is valid to avoid null reference errors
        if (player != null)
        {
            playerCollider = player.GetComponent<Collider2D>();// Get the Collider2D component of the player for line of sight checks
        }

        patrolTarget = GetNewPatrolTarget();// Set the initial patrol target to a random point within the patrol radius

        currentHealth = maxHealth;

        lastPosition = transform.position;// Initialize lastPosition to the enemy's starting position for movement tracking and stuck detection
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

    // FixedUpdate is called at a fixed interval and is used for physics updates, handling movement, detection, and behavior changes based on the player's presence
    private void FixedUpdate()
    {
        // If the player reference is not assigned, try to find the player GameObject by tag and assign its Transform to the player reference for targeting and line of sight checks; if the player GameObject cannot be found, return early to prevent null reference errors and allow the enemy to continue patrolling without targeting the player
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");// Try to find the player GameObject by tag if the player reference is not assigned

            if (playerObj == null)
            {
                return;// If the player GameObject cannot be found, return early to prevent null reference errors and allow the enemy to continue patrolling without targeting the player
            }

            player = playerObj.transform;// If the player GameObject is found, assign its Transform to the player reference for targeting and line of sight checks
        }
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);// Calculate the distance from the enemy to the player
        bool detectedNow = distanceToPlayer <= detectionRange && CanSeePlayer();// Check if the player is within the detection range and update the isPlayerDetected flag accordingly

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

            // Check if the player component exists and if there are contact points in the collision before applying damage and knockback to the player
            if (collision.contactCount > 0)
            {
                ContactPoint2D contact = collision.GetContact(0); // Get the first contact point of the collision

                Vector2 knockbackDirection = contact.normal; // The normal of the contact point will be used as the knockback direction

                Vector2 enemyToPlayer = (collision.transform.position - transform.position).normalized; // Calculate the direction from the enemy to the player

                // Check if the knockback direction is pointing towards the enemy, if so, invert it to ensure the player is knocked back away from the enemy
                if (Vector2.Dot(knockbackDirection, enemyToPlayer) < 0)
                {
                    knockbackDirection = -knockbackDirection; // Invert the knockback direction if it's pointing towards the enemy
                }

                PlayerHealth.instance.TakeDamage(knockbackDirection, attackDamage);// Apply damage to the player and knock them back in the calculated direction
            }

        }
    }

    // Method to handle patrolling behavior when the player is not detected, moving towards a patrol target and checking for obstacles and stuck conditions
    private void Patrol()
    {
        float distanceToTarget = Mathf.Abs(transform.position.x - patrolTarget.x);// Calculate the distance from the enemy to the current patrol target

        // Check if the enemy is far from the patrol target or if there are obstacles in the horizontal path towards the patrol target, and if so, get a new patrol target to try to move towards
        if (distanceToTarget < 0.1f || !IsHorizontalPathClear(patrolTarget.x))
        {
            patrolTarget = GetNewPatrolTarget();// If the enemy is far from the patrol target or if there are obstacles in the horizontal path towards the patrol target, get a new patrol target to try to move towards

            stuckTimer = 0f;// Reset the stuck timer when getting a new patrol target to prevent it from being considered stuck immediately after changing targets

            return;// Return early to allow the enemy to start moving towards the new patrol target in the next FixedUpdate
        }

        float directionX = Mathf.Sign(patrolTarget.x - transform.position.x);// Calculate the horizontal direction towards the patrol target (1 for right, -1 for left)

        // Check if there is a wall or obstacle directly ahead in the direction of movement towards the patrol target, and if so, get a new patrol target to try to move towards and stop horizontal movement to prevent the enemy from trying to move through the wall or obstacle
        if (IsColliderBlockedAhead(directionX))
        {
            patrolTarget = GetNewPatrolTarget();// If there is a wall ahead in the direction of movement towards the patrol target, get a new patrol target to try to move towards

            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);// Stop horizontal movement when a wall is detected ahead to prevent the enemy from trying to move through the wall

            stuckTimer = 0f;// Reset the stuck timer when a wall is detected ahead to prevent it from being considered stuck immediately after hitting a wall

            return;// Return early to allow the enemy to start moving towards the new patrol target in the next FixedUpdate
        }

        rb.linearVelocity = new Vector2(directionX * moveSpeed, rb.linearVelocity.y);// Move towards the patrol target at the specified move speed in the horizontal direction while maintaining the current vertical velocity

        CheckStuck(distanceToTarget);// Check if the enemy is stuck when trying to move towards the patrol target and get a new patrol target if it has been stuck for too long
    }

    // Method to handle chasing behavior when the player is detected
    private void ChasePlayer()
    {
        float directionX = Mathf.Sign(player.position.x - transform.position.x);// Calculate the horizontal direction towards the player (1 for right, -1 for left)

        // Check if there is a wall or obstacle directly ahead in the direction of movement towards the player, and if so, stop chasing and patrolling, get a new center position for patrolling, and return early to prevent the enemy from trying to move through the wall or obstacle
        if (IsColliderBlockedAhead(directionX))
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);// Stop horizontal movement when a collider is detected ahead to prevent the enemy from trying to move through the collider

            isChasing = false;// Stop chasing the player if a collider is detected ahead to prevent the enemy from trying to move through the collider

            isPatrolling = true;// Start patrolling again if a collider is detected ahead to allow the enemy to continue moving and avoid getting stuck when trying to chase the player through a wall or obstacle

            GetNewCenter();// Get a new center position for patrolling when the player is lost after being detected to allow the enemy to start patrolling around its current position

            return;// Return early to allow the enemy to start moving towards the player again in the next FixedUpdate if the path is clear
        }

        rb.linearVelocity = new Vector2(directionX * attackSpeed, rb.linearVelocity.y);// Move towards the player at the specified attack speed in the horizontal direction while maintaining the current vertical velocity
    }

    // Method to get a new random patrol target within the patrol radius around the center position
    private Vector2 GetNewPatrolTarget()
    {
        // Try to find a valid patrol target that is not blocked by obstacles, and if a valid target cannot be found after the specified number of attempts, return the current position as a fallback
        for (int i = 0; i < maxPatrolTargetAttempts; i++)
        {
            float randomX = Random.Range(-patrolRadius, patrolRadius);// Generate a random X offset within the patrol radius
            float candidateX = centerPosition.x + randomX;// Calculate the target X position for patrolling based on the center position and the random offset

            // Check if the horizontal path towards the candidate patrol target is clear of obstacles before returning it as a valid patrol target; if not, continue trying to find a valid patrol target until the maximum number of attempts is reached
            if (IsHorizontalPathClear(candidateX))
            {
                return new Vector2(candidateX, transform.position.y);// If the horizontal path towards the candidate patrol target is clear of obstacles, return the candidate patrol target as a valid patrol target
            }
        }

        return transform.position;// If a valid patrol target cannot be found after the specified number of attempts, return the current position as a fallback to prevent the enemy from trying to move towards an invalid target
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
            Vector3 newScale = graphicsOriginalScale;// Start with the original scale of the graphics Transform

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

    // Method to check if the enemy has a clear line of sight to the player, used in the detection logic to determine if the player is detected based on distance and line of sight
    private Vector2 GetSightOriginPosition()
    {
        if (sightOrigin != null)
        {
            return sightOrigin.position;// Return the position of the sightOrigin Transform if it is assigned, used as the origin point for line of sight checks
        }

        if (boxCollider != null)
        {
            return boxCollider.bounds.center;// If sightOrigin is not assigned, return the center of the BoxCollider2D bounds as the origin point for line of sight checks
        }

        return transform.position;// If neither sightOrigin nor boxCollider is assigned, return the position of the enemy GameObject as a fallback for the origin point for line of sight checks
    }

    // Method to get the target position for line of sight checks towards the player, used in the detection logic to determine if the player is detected based on distance and line of sight
    private Vector2 GetPlayerTargetPosition()
    {
        if (playerCollider != null)
        {
            return playerCollider.bounds.center;// Return the center of the player's Collider2D bounds as the target position for line of sight checks
        }

        return player.position;// If playerCollider is not assigned, return the position of the player's Transform as a fallback for the target position for line of sight checks
    }

    // Method to check if the enemy has a clear line of sight to the player by performing a linecast from the sight origin to the player's position and checking for obstacles in between
    private bool CanSeePlayer()
    {
        if (player == null)
        {
            return false;// If the player reference is not assigned, return false to indicate that the enemy cannot see the player
        }

        Vector2 origin = GetSightOriginPosition();// Get the origin position for line of sight checks
        Vector2 target = GetPlayerTargetPosition();// Get the target position for line of sight checks

        RaycastHit2D hit = Physics2D.Linecast(origin, target, obstacleLayer);// Perform a linecast from the origin to the target using the specified obstacle layer to check for obstacles between the enemy and the player

        return hit.collider == null;// If the linecast does not hit any colliders, return true to indicate that the enemy has a clear line of sight to the player; otherwise, return false
    }

    // Method to check if there are any obstacles in the horizontal path towards a target X position, used in the patrol logic to ensure that the enemy does not try to patrol through walls or obstacles
    private bool IsHorizontalPathClear(float targetX)
    {
        Vector2 origin = GetSightOriginPosition();// Get the origin position for horizontal path checks
        Vector2 target = new Vector2(targetX, origin.y);// Get the target position for horizontal path checks

        RaycastHit2D hit = Physics2D.Linecast(origin, target, obstacleLayer);// Perform a linecast from the origin to the target using the specified obstacle layer to check for obstacles in the horizontal path towards the target X position

        return hit.collider == null;// If the linecast does not hit any colliders, return true to indicate that the horizontal path is clear; otherwise, return false
    }

    // Method to check if there is a wall or obstacle directly ahead in the direction of movement, used in the patrol and chase logic to prevent the enemy from trying to move through walls or obstacles
    private bool IsWallAhead(float directionX)
    {
        if (Mathf.Approximately(directionX, 0))
        {
            return false;// If the directionX is approximately zero, return false to indicate that there is no wall ahead since there is no horizontal movement
        }

        Vector2 origin = GetSightOriginPosition();// Get the origin position for wall checks
        Vector2 direction = Vector2.right * directionX;// Get the target direction for wall checks based on the horizontal movement direction (1 for right, -1 for left)

        RaycastHit2D hit = Physics2D.Raycast(origin, direction,wallCheckDistance, obstacleLayer);// Perform a raycast from the origin in the specified direction for the specified distance using the obstacle layer to check for walls or obstacles ahead

        return hit.collider != null;// If the linecast hits a collider, return true to indicate that there is a wall ahead; otherwise, return false
    }

    // Method to check if the enemy is stuck when trying to move towards the patrol target, and if it has been stuck for longer than the specified duration, get a new patrol target to try to move towards
    private void CheckStuck(float distanceToTarget)
    {
        float horizontalMovement = Mathf.Abs(rb.position.x - lastPosition.x);// Calculate the horizontal movement since the last FixedUpdate

        // If the enemy is trying to move towards the target but has not moved enough, increment the stuck timer; otherwise, reset the stuck timer
        if (distanceToTarget > 0.1f && horizontalMovement < minimumMovementPerFixedUpdate)
        {
            stuckTimer += Time.fixedDeltaTime;// If the enemy is trying to move towards the target but has not moved enough, increment the stuck timer
        }

        // If the enemy has moved enough towards the target, reset the stuck timer to prevent it from being considered stuck
        else
        {
            stuckTimer = 0f;// If the enemy has moved enough towards the target, reset the stuck timer
        }

        // If the enemy has been stuck for longer than the specified duration, get a new patrol target to try to move towards and reset the stuck timer
        if (stuckTimer >= stuckDuration)
        {
            patrolTarget = GetNewPatrolTarget();// If the enemy has been stuck for too long, get a new patrol target to try to move towards

            stuckTimer = 0f;// Reset the stuck timer after getting a new patrol target
        }

        lastPosition = rb.position;// Update the lastPosition to the current position for the next FixedUpdate
    }

    private bool IsColliderBlockedAhead(float directionX)
    {
        if (Mathf.Approximately(directionX, 0) || boxCollider == null)
        {
            return false;// If the directionX is approximately zero, return false to indicate that there is no collider ahead since there is no horizontal movement
        }

        Vector2 direction = new Vector2(Mathf.Sign(directionX), 0);// Get the target direction for collider checks based on the horizontal movement direction (1 for right, -1 for left)

        int hitCount = boxCollider.Cast(direction, obstacleContactFilter, obstacleHits, colliderCheckDistance);// Perform a BoxCast from the BoxCollider2D in the specified direction for the specified distance using the obstacle contact filter to check for colliders ahead

        return hitCount > 0;// If the BoxCast hits any colliders, return true to indicate that there is a collider ahead; otherwise, return false
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

        if (player != null)
        {
            Vector2 origin = sightOrigin != null ? sightOrigin.position : transform.position;

            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(origin, player.position);
        }

        Vector2 wallOrigin = sightOrigin != null ? sightOrigin.position : transform.position;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(wallOrigin, wallOrigin + Vector2.right * wallCheckDistance);
        Gizmos.DrawLine(wallOrigin, wallOrigin + Vector2.left * wallCheckDistance);
    }
}
