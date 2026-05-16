using UnityEngine;

public class RoundController : MonoBehaviour
{
    [SerializeField] private float roundDuration = 60f; // Duration of each round in seconds
    [SerializeField] private int roundDamage = 1; // Amount of damage the round will inflict on enemies it collides with, can be set in the Inspector
    [SerializeField] private int emergencyMultiplier = 2; // Multiplier for round duration during emergency mode, can be set in the Inspector
    [SerializeField] private float emergencyDivider = 2f; // Divider for round damage during emergency mode, can be set in the Inspector

    private bool isEnmergencyOn = false;// bool to track if emergency mode has been activated, used to prevent repeatedly applying the emergency multiplier
    private EnemyController enemyController; // Reference to the EnemyController script to manage enemy behavior during the round

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // destroy the round after the specified duration
        Destroy(gameObject, roundDuration); // Destroy the round after the specified duration
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the player is in emergency mode and adjust round behavior accordingly
        if (PlayerHealth.instance.isEmergencyMode && !isEnmergencyOn)
        {
            EmergencyRound(emergencyDivider); // If the player is in emergency mode, call the EmergencyRound method to adjust round duration and damage accordingly
            isEnmergencyOn = true; // Set the emergency mode flag to true to prevent repeatedly applying the emergency multiplier
        }
    }

    // This method is called when the round collides with another collider
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            enemyController = collision.gameObject.GetComponent<EnemyController>(); // Get the EnemyController component from the collided enemy

            if (!enemyController.damage)
            {
                enemyController.TakeDamage(roundDamage); // Apply damage to the enemy based on the roundDamage value
            }

            Destroy(gameObject); // Destroy the round immediately if an enemy collides with it
        }
    }

    // This method can be called to activate emergency mode, which reduces the round duration to make it more challenging for the player
    public void EmergencyRound(float multiplier)
    {
        roundDuration /= multiplier; // Reduce the round duration by the specified multiplier to make it more challenging during emergency mode

        roundDamage *= emergencyMultiplier; // Increase the round damage by the emergency multiplier to make it more powerful during emergency mode
    }
}
