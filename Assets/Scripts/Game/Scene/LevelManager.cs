using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using GameAnalyticsSDK;

public class LevelManager : MonoBehaviour
{
    private VisualElement gameUI;// Reference to the game UI element, which can be used to show or hide the UI when pausing or resuming the game
    public static bool isPaused = false;// Static flag to track whether the game is currently paused, accessible from other scripts
    public static float startTime;// Variable to store the time when the level started, can be used for tracking level duration or analytics
    private bool abandonRegistered = false;// Flag to track whether the player has abandoned the level, used to prevent multiple abandonment events from being logged

    [Header("Utilities")]
    [SerializeField] private UIDocument uiDocument;// Reference to the UIDocument component that contains the game UI

    // Awake is called when the script instance is being loaded
    void Awake()
    {
        // If the UIDocument reference is not set in the inspector, try to get it from the current GameObject
        if (uiDocument == null)
        {
            uiDocument = GetComponent<UIDocument>();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1f;// Ensure the game starts with normal time scale

        isPaused = false;// Set the paused flag to false when the level starts

        startTime = Time.time;// Record the time when the level started, which can be used for tracking how long the player spends in the level

        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, "Level_1");// Log a new progression event to GameAnalytics indicating that the player has started Level 1 again
    }

    // OnEnable is called when the object becomes enabled and active
    void OnEnable()
    {
        gameUI = uiDocument.rootVisualElement.Q<VisualElement>("GameUI");// Get the game UI element by name from the root visual element of the UI document

        gameUI.style.display = DisplayStyle.Flex;// Ensure the game UI is visible when the level starts
    }

    // This method can be called to pause the game, typically when the player opens the pause menu or when the player dies
    public void Pause()
    {
        isPaused = true;// Set the paused flag to true

        Time.timeScale = 0f;// Freeze the game by setting time scale to 0
    }

    // This method can be called to resume the game, typically when the player closes the pause menu or restarts the level
    public void Resume()
    {
        isPaused = false;// Set the paused flag to false

        Time.timeScale = 1f;// Resume the game by setting time scale back to 1
    }

    // This method can be called to restart the current level, typically when the player clicks the restart button in the game over menu
    public void Restart()
    {
        RegisterAbandon();// Call the method to register an abandoned attempt before restarting the level

        isPaused = false;// Set the paused flag to false to ensure the game is not paused when restarting

        Scene currentScene = SceneManager.GetActiveScene();// Get the currently active scene
        SceneManager.LoadScene(currentScene.name);// Reload the current scene
    }

    // This method can be called to return to the main menu scene
    public void MainMenu()
    {
        RegisterAbandon();// Call the method to register an abandoned attempt before returning to the main menu

        Time.timeScale = 1f;// Ensure the time scale is reset to normal before loading the main menu
        SceneManager.LoadScene("MainMenu");// Load the main menu scene by name
    }

    // This method can be called to exit the game application
    public void Exit()
    {
        RegisterAbandon();// Call the method to register an abandoned attempt before exiting the game

        Time.timeScale = 1f;// Ensure the time scale is reset to normal before exiting the game
        Application.Quit();// Quit the application when the exit button is clicked
        Debug.Log("Quit game");// Log a message to the console for debugging purposes, since Application.Quit() does not work in the editor
    }

    // Record an unfinished attempt only once.
    private void RegisterAbandon()
    {
        // Prevent duplicate abandonment events.
        if (abandonRegistered)
            return;

        // Ignore attempts that already ended in victory or defeat.
        if (PlayerHealth.instance != null &&
            PlayerHealth.instance.isDead)
            return;

        if (ScoreManager.Instance != null &&
            ScoreManager.Instance.levelCompleted)
            return;

        abandonRegistered = true;

        // Calculate the time spent playing before leaving.
        float durationSeconds = Time.time - startTime;

        // Track an abandoned attempt and its duration.
        GameAnalytics.NewDesignEvent(
            "Level_1:Duration:Abandon", durationSeconds
        );
    }
}
