using UnityEngine;

public class RoundController : MonoBehaviour
{
    [SerializeField] private float roundDuration = 60f; // Duration of each round in seconds

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
}
