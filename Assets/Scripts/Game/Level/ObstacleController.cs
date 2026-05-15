using UnityEngine;

public class ObstacleController : MonoBehaviour
{
    [Header("Obstacle Settings")]
    [SerializeField] private int damage = 1;

    // This method is called when the obstacle collides with another object
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Check if the player component exists and if there are contact points in the collision
            if (collision.contactCount > 0)
            {
                ContactPoint2D contact = collision.GetContact(0); // Get the first contact point of the collision

                Vector2 knockbackDirection = contact.normal; // The normal of the contact point will be used as the knockback direction

                Vector2 obstacleToPlayer = (collision.transform.position - transform.position).normalized; // Calculate the direction from the obstacle to the player

                // Check if the knockback direction is pointing towards the obstacle, if so, invert it to ensure the player is knocked back away from the obstacle
                if (Vector2.Dot(knockbackDirection, obstacleToPlayer) < 0)
                {
                    knockbackDirection = -knockbackDirection; // Invert the knockback direction if it's pointing towards the obstacle
                }

                PlayerHealth.instance.TakeDamage(knockbackDirection, damage);// Optionally, you can also add some visual or sound effects here to indicate the collision
            }
        }
    }
}
