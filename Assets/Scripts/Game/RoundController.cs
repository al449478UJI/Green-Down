using UnityEngine;

public class RoundController : MonoBehaviour
{
    [SerializeField] private float roundDuration = 60f; // Duration of each round in seconds
    [SerializeField] private int roundDamage = 1; // Amount of damage the round will inflict on enemies it collides with, can be set in the Inspector
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
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            enemyController = collision.gameObject.GetComponent<EnemyController>(); // Get the EnemyController component from the collided enemy

            if(!enemyController.damage)
            {
                enemyController.TakeDamage(roundDamage); // Apply damage to the enemy based on the roundDamage value
            }

            Destroy(gameObject); // Destroy the round immediately if an enemy collides with it
        }
    }
}
