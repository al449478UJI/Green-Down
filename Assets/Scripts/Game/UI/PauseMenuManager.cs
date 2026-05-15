using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;

public class PauseMenuManager : MonoBehaviour
{
    private VisualElement pauseMenu;// Reference to the pause menu UI element
    private Button resumeButton;// Reference to the resume button in the pause menu
    private Button restartButton;// Reference to the restart button in the pause menu
    private Button exitButton;// Reference to the exit button in the pause menu
    private Button mainMenuButton;// Reference to the main menu button in the pause menu
    private bool isPaused = false;// Flag to track whether the game is currently paused

    [Header("Utilities")]
    [SerializeField] private UIDocument uiDocument;// Reference to the UIDocument component that contains the pause menu UI
    private PlayerInput playerInput;// Reference to the PlayerInput component for handling input

    // Awake is called when the script instance is being loaded
    void Awake()
    {
        // If the UIDocument reference is not set in the inspector, try to get it from the current GameObject
        if (uiDocument == null)
        {
            uiDocument = GetComponent<UIDocument>();
        }

        if (playerInput == null)
        {
            playerInput = GetComponent<PlayerInput>();
        }
    }

    // OnEnable is called when the object becomes enabled and active
    private void OnEnable()
    {
        VisualElement root = uiDocument.rootVisualElement;// Get the root visual element of the UI document

        pauseMenu = root.Q<VisualElement>("PauseMenu");// Find the pause menu element by name
        resumeButton = root.Q<Button>("ResumeButton");// Find the resume button by name
        restartButton = root.Q<Button>("RestartButton");// Find the restart button by name
        exitButton = root.Q<Button>("ExitButton");// Find the exit button by name
        mainMenuButton = root.Q<Button>("MainMenuButton");// Find the main menu button by name

        resumeButton.clicked += ResumeGame;// Add a click event listener to the resume button
        restartButton.clicked += RestartGame;// Add a click event listener to the restart button
        exitButton.clicked += ExitGame;// Add a click event listener to the exit button
        //mainMenuButton.clicked += ReturnToMainMenu;// Add a click event listener to the main menu button

        HidePauseMenu();// Initially hide the pause menu when the game starts
    }

    // OnDisable is called when the behaviour becomes disabled or inactive
    private void OnDisable()
    {
        // Remove the click event listeners when the object is disabled to prevent memory leaks
        resumeButton.clicked -= ResumeGame;
        restartButton.clicked -= RestartGame;
        exitButton.clicked -= ExitGame;
        //mainMenuButton.clicked -= ReturnToMainMenu;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    // This method is called when the pause input action is triggered
    private void OnPauseBack(InputValue button)
    {
        //Check if the pause button is pressed and toggle the pause state accordingly
        if (button.isPressed)
        {
            TogglePause(); // Call the method to toggle the pause state when the pause input action is triggered
        }
    }

    private void TogglePause()
    {
        if (isPaused)
        {
            ResumeGame();// If the game is currently paused, resume it

            PlayerAttack.instance.enabled = true;// Enable the PlayerAttack script to allow the player to attack again when the game is resumed
        }
        else
        {
            PlayerAttack.instance.enabled = false;// Disable the PlayerAttack script to prevent the player from attacking while the game is paused

            PauseGame();// If the game is currently running, pause it
        }
    }

    // Method to pause the game and show the pause menu
    private void PauseGame()
    {
        isPaused = true;// Set the paused flag to true

        Time.timeScale = 0f;// Freeze the game by setting time scale to 0

        ShowPauseMenu();// Show the pause menu UI
    }

    // Method to resume the game and hide the pause menu
    private void ResumeGame()
    {
        isPaused = false;// Set the paused flag to false

        Time.timeScale = 1f;// Resume the game by setting time scale back to 1

        HidePauseMenu();// Hide the pause menu UI
    }

    // Method to show the pause menu
    private void ShowPauseMenu()
    {
        pauseMenu.style.display = DisplayStyle.Flex;// Set the display style of the pause menu to flex to make it visible
    }

    // Method to hide the pause menu
    private void HidePauseMenu()
    {
        pauseMenu.style.display = DisplayStyle.None;// Set the display style of the pause menu to none to hide it
    }

    //Method to restart the game when the restart button is clicked
    private void RestartGame()
    {
        Time.timeScale = 1f;// Ensure the time scale is reset to normal before restarting the level

        Scene currentScene = SceneManager.GetActiveScene();// Get the currently active scene
        SceneManager.LoadScene(currentScene.name);// Reload the current scene
    }

    //Method to exit the game when the exit button is clicked
    private void ExitGame()
    {
        Time.timeScale = 1f;// Ensure the time scale is reset to normal before exiting the game
        Application.Quit();// Quit the application when the exit button is clicked
        Debug.Log("Quit game");
    }
}
