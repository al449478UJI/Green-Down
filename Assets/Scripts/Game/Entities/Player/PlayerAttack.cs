using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack settings")]
    [SerializeField] private Rigidbody2D projectile;// Serialized fields allow you to set these variables in the Unity Editor
    [SerializeField] private Transform endBarrel;// The endBarrel is the point from which the projectile will be instantiated and shot
    [SerializeField] private float bulletSpeed = 10f;// The speed at which the projectile
    [SerializeField] private float fireRate = 0.5f;// The rate at which the player can shoot (in seconds)
    private float nextFireTime = 0f;// The time at which the player can shoot again

    [Header("Emergency Mode")]
    [SerializeField] private float emergencyMultyplier = 1.5f;// Multiplier for bullet speed and fire rate when in emergency mode, can be set in the Inspector

    [Header("Utility")]
    public static PlayerAttack instance;// Static instance of PlayerAttack for easy access from other scripts, implementing a singleton pattern

    // Awake is called when the script instance is being loaded
    void Awake()
    {
        // Set up the singleton pattern for PlayerAttack
        if (instance == null)
        {
            instance = this;// Set the static instance to this instance of PlayerAttack for easy access from other scripts
        }
        else
        {
            Destroy(gameObject);// If an instance already exists, destroy this duplicate to enforce the singleton pattern
        }
    }

    // This method is called when the shoot input action is triggered
    public void OnShoot(InputValue button)
    {
        // Check if the shoot button is pressed and if the current time is greater than or equal to the next allowed fire time
        if (button.isPressed && Time.time >= nextFireTime && !LevelManager.isPaused && !PlayerHealth.instance.isDead)
        {
            Shoot(); // Call the Shoot method to instantiate and shoot the projectile

            nextFireTime = Time.time + fireRate; // Update the nextFireTime to enforce the fire rate
        }
    }

    // This method handles the instantiation and shooting of the projectile
    private void Shoot()
    {
        // Instantiate a new projectile at the position of the endBarrel with a rotation of 90 degrees on the Z-axis
        Rigidbody2D round;
        round = Instantiate(projectile, endBarrel.position, Quaternion.Euler(0, 0, 90));

        // Apply a force to the projectile in the direction the player is facing
        if (PlayerMovement.instance.isFacingRight)
        {
            round.AddForce(endBarrel.right * bulletSpeed, ForceMode2D.Impulse);
        }

        // If the player is facing left, apply the force in the opposite direction
        else if (!PlayerMovement.instance.isFacingRight)
        {
            round.AddForce(endBarrel.right * (bulletSpeed * -1), ForceMode2D.Impulse);
        }
    }

    // This method can be called to set the player into emergency mode, which increases bullet speed and decreases fire rate to make the player more powerful when health is low
    public void SetEmergencyMode()
    {
        bulletSpeed *= emergencyMultyplier;// Increase bullet speed by 50% in emergency mode to make the player more powerful when health is low

        fireRate *= emergencyMultyplier;// Decrease fire rate by 25% in emergency mode to allow the player to shoot more frequently when health is low
    }
}
