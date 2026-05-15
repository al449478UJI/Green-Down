using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class LevelManager : MonoBehaviour
{
    private VisualElement gameUI;// Reference to the game UI element, which can be used to show or hide the UI when pausing or resuming the game
    public static bool isPaused = false;// Static flag to track whether the game is currently paused, accessible from other scripts

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

    // OnEnable is called when the object becomes enabled and active
    void OnEnable()
    {
        gameUI = uiDocument.rootVisualElement.Q<VisualElement>("GameUI");// Get the game UI element by name from the root visual element of the UI document

        gameUI.style.display = DisplayStyle.Flex;// Ensure the game UI is visible when the level starts
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Pause()
    {
        isPaused = true;// Set the paused flag to true

        Time.timeScale = 0f;// Freeze the game by setting time scale to 0
    }

    public void Resume()
    {
        isPaused = false;// Set the paused flag to false

        Time.timeScale = 1f;// Resume the game by setting time scale back to 1
    }

    public void Restart()
    {
        Time.timeScale = 1f;// Ensure the time scale is reset to normal before restarting the level

        Scene currentScene = SceneManager.GetActiveScene();// Get the currently active scene
        SceneManager.LoadScene(currentScene.name);// Reload the current scene
    }

    public void Exit()
    {
        Time.timeScale = 1f;// Ensure the time scale is reset to normal before exiting the game
        Application.Quit();// Quit the application when the exit button is clicked
        Debug.Log("Quit game");
    }
}
