using GameAnalyticsSDK;
using UnityEngine;

public class GameAnalyticsInitializer : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Initialize the analytics SDK once at game startup.
        GameAnalytics.Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
