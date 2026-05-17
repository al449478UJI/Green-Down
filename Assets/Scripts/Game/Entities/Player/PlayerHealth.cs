using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 5;// Maximum health for the player, can be set in the Inspector

    private int currentHealth;// Current health of the player, initialized in Start() to maxHealth
    public bool isDead = false;// Static flag to track whether the player is currently dead, can be used to prevent taking damage or triggering death multiple times

    [Header("Damage Effects")]
    [SerializeField] private float invulnerableTime = 1.0f;// Duration of invulnerability after taking damage, can be set in the Inspector
    [SerializeField] private float knockbackForce = 5f;// Force of knockback applied to the player when taking damage, can be set in the Inspector
    public bool isInvulnerable = false;// bool to track if the player is currently invulnerable, used to prevent taking damage multiple times in quick succession


    [Header("Emergency Mode")]
    [SerializeField] private GameObject flash;
    [SerializeField] private float iFramesFlashFrequency = 0.1f;// Frequency of flashing effect during emergency mode, can be set in the Inspector, used to visually indicate low health
    [SerializeField] private int flashAmount = 5;// Number of times to flash during emergency mode, can be set in the Inspector, used to visually indicate low health

    public bool isEmergencyMode = false;// bool to track if the player is in emergency mode, can be used to trigger different behavior or animations when health is low

    [Header("Utility")]
    [SerializeField] private Animator animator;// Animator component for controlling animations, can be set in the Inspector

    private Rigidbody2D rb;// Reference to the player's Rigidbody2D component for applying knockback, can be set in the Inspector
    public static PlayerHealth instance;// Static instance of PlayerHealth for easy access from other scripts, implementing a singleton pattern

    // Awake is called when the script instance is being loaded
    void Awake()
    {
        // Implementing singleton pattern to ensure only one instance of PlayerHealth exists and can be easily accessed from other scripts
        if (instance == null)
        {
            instance = this;// Set the static instance to this instance of PlayerHealth if it hasn't been set yet
        }
        else
        {
            Destroy(gameObject);// Destroy this GameObject if another instance of PlayerHealth already exists to enforce the singleton pattern
        }

        rb = GetComponent<Rigidbody2D>();// Get the Rigidbody2D component attached to the player GameObject for applying knockback when taking damage

        if (flash == null)
        {
            flash = GameObject.Find("DamageFlash");
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        flash.SetActive(false);// Ensure the flash GameObject is initially inactive
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentHealth < 3 && !isEmergencyMode)
        {
            PlayerHealth.instance.SetEmergencyMode();
        }

        if (currentHealth <= 0)
        {
            Die();
        }

        animator.SetBool("dead", isDead);// Update the "dead" parameter in the Animator based on whether the player is currently dead to control animations
        animator.SetBool("damage", isInvulnerable);// Update the "damage" parameter in the Animator based on whether the player is currently invulnerable to control animations
    }

    // Method to apply damage to the player
    public void TakeDamage(Vector2 direction, int amount)
    {
        // Ignore damage if the player is dead or invurnerable
        if (isDead || isInvulnerable)
        {
            return;
        }

        currentHealth -= amount;// Reduce current health by the damage amount

        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);// Clamp current health to ensure it doesn't go below 0 or above maxHealth

        HealthBarManager.instance.UpdateHealthBard(currentHealth);// Update the health bar UI to reflect the new current health

        Vector2 knockbackDirection = direction.normalized;// Normalize the direction vector to get the direction of knockback without affecting its magnitude

        rb.linearVelocity = Vector2.zero;// Reset the player's velocity to ensure consistent knockback behavior regardless of current movement

        rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);// Apply knockback force to the player's Rigidbody2D

        // Start the invulnerability coroutine if the player is still alive after taking damage
        if (currentHealth > 0)
        {
            StartCoroutine(InvulnerabilityCoroutine());// Start the invulnerability coroutine after taking damage
        }
    }

    // Coroutine to handle the player's invulnerability after taking damage
    private IEnumerator InvulnerabilityCoroutine()
    {
        isInvulnerable = true;// Start invulnerability

        PlayerMovement.instance.enabled = false;// Disable movement while invulnerable

        PlayerAttack.instance.enabled = false;// Disable attacking while invulnerable

        yield return new WaitForSeconds(invulnerableTime);// Wait for the duration of invulnerability

        PlayerMovement.instance.enabled = true;// Re-enable movement after invulnerability

        PlayerAttack.instance.enabled = true;// Re-enable attacking after invulnerability

        isInvulnerable = false;// End invilnerability
    }

    //  Method to handle the player's death
    public void Die()
    {
        // Ignore if the player is already dead to prevent multiple death triggers
        if (isDead)
        {
            return;
        }

        isDead = true;// Set the isDead flag to true to indicate the player is now dead

        // Check if the player is grounded when they die to determine whether to disable the Rigidbody2D simulation immediately or not
        if (PlayerMovement.instance.isGrounded)
        {
            rb.simulated = false;// Disable the Rigidbody2D simulation to prevent further physics interactions when the player is dead and grounded
        }
    }

    // Method to set the player into emergency mode, which can be used to trigger different behavior or animations when health is low
    public void SetEmergencyMode()
    {
        isEmergencyMode = true;// Set the emergency mode flag to true to indicate the player is in emergency mode

        StartCoroutine(FlashCoroutine());// Start the flashing effect coroutine to visually indicate that the player is in emergency mode
    }

    // Coroutine to handle the flashing effect when the player is in emergency mode, which can be used to visually indicate low health
    private IEnumerator FlashCoroutine()
    {
        // Flash the player a certain number of times based on the flashAmount variable, with a delay between each flash based on the iFramesFlashFrequency variable
        for (int i = 0; i < flashAmount; i++)
        {
            flash.SetActive(true);
            yield return new WaitForSeconds(iFramesFlashFrequency);
            flash.SetActive(false);
            yield return new WaitForSeconds(iFramesFlashFrequency);
        }
    }
}
