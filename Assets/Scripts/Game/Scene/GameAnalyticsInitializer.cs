using GameAnalyticsSDK;
using UnityEngine;

public class GameAnalyticsInitializer : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Log the initialization attempt.
        Debug.Log("[GA TEST] Initializing GameAnalytics");

        // Initialize the analytics SDK once at game startup.
        GameAnalytics.Initialize();

        // Confirm that the initialization method was called.
        Debug.Log("[GA TEST] Initialize() was called");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
