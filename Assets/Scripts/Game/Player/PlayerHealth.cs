using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 5;// Maximum health for the player, can be set in the Inspector
    private int currentHealth;// Current health of the player, initialized in Start() to maxHealth
    private bool isDead = false;// bool to track if the player is currently dead, used to prevent multiple death triggers and to control animations

    [Header("Damage Effects")]
    [SerializeField] private float invulnerableTime = 1.0f;// Duration of invulnerability after taking damage, can be set in the Inspector
    [SerializeField] private float knockbackForce = 5f;// Force of knockback applied to the player when taking damage, can be set in the Inspector
    public bool isInvulnerable = false;// bool to track if the player is currently invulnerable, used to prevent taking damage multiple times in quick succession

    [Header("Emergency Mode")]
    [SerializeField] private GameObject flash;
    [SerializeField] private float iFramesFlashFrequency = 0.1f;// Frequency of flashing effect during emergency mode, can be set in the Inspector, used to visually indicate low health
    [SerializeField] private int flashAmount = 5;// Number of times to flash during emergency mode, can be set in the Inspector, used to visually indicate low health
    private bool isEmergencyMode = false;// bool to track if the player is in emergency mode, can be used to trigger different behavior or animations when health is low

    [Header("Utility")]
    [SerializeField] private Animator animator;// Animator component for controlling animations, can be set in the Inspector
    [SerializeField] private PlayerMovement movement;// Reference to the PlayerMovement script, used to disable movement when the player dies, can be set in the Inspector
    [SerializeField] private PlayerAttack attack;// Reference to the PlayerAttack script, used to disable attacking when the player dies, can be set in the Inspector
    [SerializeField] private PlayerHealth Health;// Reference to the PlayerHealth script, used to set emergency mode when health is low, can be set in the Inspector
    [SerializeField] private Rigidbody2D rb;// Reference to the player's Rigidbody2D component for applying knockback, can be set in the Inspector

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
            Health.SetEmergencyMode();
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

        Vector2 knockback = new Vector2((transform.position.x - direction.x)*knockbackForce,1*knockbackForce);// Calculate knockback force based on the direction of the attack, can be adjusted for different feel
        rb.AddForce(knockback, ForceMode2D.Impulse);// Apply knockback force to the player's Rigidbody2D

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

        movement.enabled = false;// Disable movement while invulnerable
        attack.enabled = false;// Disable attacking while invulnerable

        yield return new WaitForSeconds(invulnerableTime);// Wait for the duration of invulnerability

        movement.enabled = true;// Re-enable movement after invulnerability
        attack.enabled = true;// Re-enable attacking after invulnerability

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

        isDead = true;
        movement.enabled = false;
        attack.enabled = false;
    }

    // Method to set the player into emergency mode, which can be used to trigger different behavior or animations when health is low
    public void SetEmergencyMode()
    {
        isEmergencyMode = true;
        movement.SetEmergencyMode();
        StartCoroutine(FlashCoroutine());
    }

    private IEnumerator FlashCoroutine()
    {
        for (int i = 0; i < flashAmount; i++)
        {
            flash.SetActive(true);
            yield return new WaitForSeconds(iFramesFlashFrequency);
            flash.SetActive(false);
            yield return new WaitForSeconds(iFramesFlashFrequency);
        }
    }
}
