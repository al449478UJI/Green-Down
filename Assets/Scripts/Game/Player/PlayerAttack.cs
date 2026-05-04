using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack settings")]
    [SerializeField] private Rigidbody2D projectile;// Serialized fields allow you to set these variables in the Unity Editor
    [SerializeField] private Transform endBarrel;// The endBarrel is the point from which the projectile will be instantiated and shot
    [SerializeField] private float bulletSpeed = 10f;// The speed at which the projectile

    [Header("Utility")]
    [SerializeField] private PlayerMovement PlayerMovement;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    // This method is called when the shoot input action is triggered
    void OnShoot(InputValue button)
    {
        if (button.isPressed)
        {
            // Instantiate a new projectile at the position of the endBarrel with a rotation of 90 degrees on the Z-axis
            Rigidbody2D round;
            round = Instantiate(projectile, endBarrel.position, Quaternion.Euler(0, 0, 90));

            // Apply a force to the projectile in the direction the player is facing
            if (PlayerMovement.isFacingRight)
            {
                round.AddForce(endBarrel.right * bulletSpeed, ForceMode2D.Impulse);
            }

            else if (!PlayerMovement.isFacingRight)
            {
                round.AddForce(endBarrel.right * (bulletSpeed * -1), ForceMode2D.Impulse);
            }
        }
    }
}
