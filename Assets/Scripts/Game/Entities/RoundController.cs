using UnityEngine;

public class RoundController : MonoBehaviour
{
    [SerializeField] private float roundDuration = 60f; // Duration of each round in seconds
    [SerializeField] private int roundDamage = 1; // Amount of damage the round will inflict on enemies it collides with, can be set in the Inspector
    [SerializeField] private int emergencyMultiplier = 2; // Multiplier for round duration during emergency mode, can be set in the Inspector
    [SerializeField] private float cameraMargin = 0.1f;// Margin for checking if the round is outside the camera's viewport, can be set in the Inspector to ensure the round is fully off-screen before destroying it

    private bool isEnmergencyOn = false;// bool to track if emergency mode has been activated, used to prevent repeatedly applying the emergency multiplier
    private EnemyController enemyController; // Reference to the EnemyController script to manage enemy behavior during the round
    private Camera mainCamera;// Reference to the main camera for checking if the round is outside the camera's viewport

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, roundDuration);// Schedule the destruction of the round GameObject after the specified round duration to prevent it from lingering indefinitely in the scene

        mainCamera = Camera.main;// Get the main camera reference for checking if the round is outside the camera's viewport
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the player is in emergency mode and adjust round behavior accordingly
        if (PlayerHealth.instance.isEmergencyMode && !isEnmergencyOn)
        {
            EmergencyRound(); // If the player is in emergency mode, call the EmergencyRound method to adjust round duration and damage accordingly
            isEnmergencyOn = true; // Set the emergency mode flag to true to prevent repeatedly applying the emergency multiplier
        }

        CheckIfOutsideCamera();// Check if the round is outside the camera's viewport and destroy it if it is to prevent rounds from lingering indefinitely off-screen
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

    // This method checks if the round is outside the camera's viewport and destroys it if it is, to prevent rounds from lingering indefinitely off-screen
    private void CheckIfOutsideCamera()
    {
        // If the main camera reference is not set, return early to avoid errors
        if (mainCamera == null)
        {
            return;
        }

        Vector3 viewportPosition = mainCamera.WorldToViewportPoint(transform.position);// Convert the round's world position to viewport coordinates (0 to 1 range)

        bool isOutsideCamera = viewportPosition.x < -cameraMargin || viewportPosition.x > 1 + cameraMargin || viewportPosition.y < -cameraMargin || viewportPosition.y > 1 + cameraMargin;// Check if the round is outside the camera's viewport with an added margin to ensure it is fully off-screen before destroying it

        // If the round is outside the camera's viewport, destroy it to prevent it from lingering indefinitely off-screen
        if (isOutsideCamera)
        {
            Destroy(gameObject);
        }
    }

    // This method is called to adjust the round's behavior when the player is in emergency mode, increasing its damage and duration to make it more powerful during emergency mode
    public void EmergencyRound()
    {
        roundDamage *= emergencyMultiplier; // Increase the round damage by the emergency multiplier to make it more powerful during emergency mode
    }
}
