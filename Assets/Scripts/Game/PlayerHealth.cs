using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 5;
    private int currentHealth;
    private bool isDead = false;

    [Header("Damage Effects")]
    [SerializeField] private float invulnerableTime = 1.0f;
    private bool isInvulnerable = false;

    [Header("Utility")]
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerMovement movement;
    [SerializeField] private PlayerAttack attack;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void TakeDamage(int amount)
    {
        // Ignore damage if the player is dead or invurnerable
        if (isDead || isInvulnerable)
        {
            return;
        }

        currentHealth -= amount;

        StartCoroutine(InvulnerabilityCoroutine());
    }

    private IEnumerator InvulnerabilityCoroutine()
    {
        isInvulnerable = true;// Start invulnerability

        yield return new WaitForSeconds(invulnerableTime);

        isInvulnerable = false;// End invilnerability
    }

    public void Die()
    {
        if (isDead)
        {
            return;
        }

        isDead = true;
        movement.enabled = false;
        attack.enabled = false;

    }
}
