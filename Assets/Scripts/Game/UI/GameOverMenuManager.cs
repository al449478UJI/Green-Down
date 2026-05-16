using UnityEngine;
using System.Collections;
using UnityEngine.UIElements;

public class GameOverMenuManager : MonoBehaviour
{
    private VisualElement gameOverMenu;// Reference to the game over menu UI element
    private Button restartButton;// Reference to the restart button in the game over menu
    private Button exitButton;// Reference to the exit button in the game over menu
    private Button mainMenuButton;// Reference to the main menu button in the game over menu

    [Header("Game Over Settings")]
    [SerializeField] private float gameOverDelay = 1.5f;// Time to wait before showing the game over menu

    private bool gameOverSequenceStarted = false;// Prevents starting the game over sequence more than once

    [Header("Utilities")]
    [SerializeField] private UIDocument uiDocument;// Reference to the UIDocument component that contains the pause menu UI
    [SerializeField] private LevelManager level;// Reference to the LevelManager script for controlling game pause and resume

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
        VisualElement root = uiDocument.rootVisualElement;// Get the root visual element of the UI document

        gameOverMenu = root.Q<VisualElement>("GameOverMenu");// Find the game over menu element by name
        restartButton = root.Q<Button>("RestartButtonGo");// Find the restart button by name
        exitButton = root.Q<Button>("ExitButtonGo");// Find the exit button by name
        mainMenuButton = root.Q<Button>("MainMenuButtonGo");// Find the main menu button by name

        restartButton.clicked += OnRestartClicked;// Add a click event listener to the restart button
        exitButton.clicked += OnExitClicked;// Add a click event listener to the exit button
        mainMenuButton.clicked += OnMainMenuClicked;// Add a click event listener to the main menu button

        gameOverMenu.style.display = DisplayStyle.None;// Initially hide the game over menu when the game starts
    }

    // OnDisable is called when the behaviour becomes disabled or inactive
    void OnDisable()
    {
        // Remove the click event listeners when the object is disabled to prevent memory leaks
        restartButton.clicked -= OnRestartClicked;
        exitButton.clicked -= OnExitClicked;
        mainMenuButton.clicked -= OnMainMenuClicked;
    }
    
    // Update is called once per frame
    void Update()
    {
        // Check if the player is dead and the game over sequence hasn't already started to prevent multiple triggers
        if (PlayerHealth.instance.isDead && !gameOverSequenceStarted)// Check if the player is dead by accessing the static isDead property in the PlayerHealth script
        {
            StartCoroutine(GameOverSequence());// Start the game over sequence coroutine if the player is dead and the sequence hasn't already started
        }

        // Check if the player is not dead and the game over menu is currently displayed, which means the player has restarted or returned to the main menu from the game over menu
        else if (!PlayerHealth.instance.isDead && gameOverMenu.style.display == DisplayStyle.Flex)// Check if the game over menu is currently displayed
        {
            level.Resume();// Resume the game if the player is not dead and the game over menu is displayed

            HideGameOverMenu();// If the player is not dead and the game over menu is displayed, hide the game over menu
        }
    }

    // This method can be called to show the game over menu when the player dies
    private void ShowGameOverMenu()
    {
        gameOverMenu.style.display = DisplayStyle.Flex;// Show the game over menu when the player dies
    }

    // This method can be called to hide the game over menu when the player restarts or exits
    private void HideGameOverMenu()
    {
        gameOverMenu.style.display = DisplayStyle.None;// Hide the game over menu when the player restarts or exits
    }

    // This method can be called to restart the level when the restart button is clicked
    private void OnRestartClicked()
    {
        level.Restart();// Call the Restart method in the LevelManager to restart the level
    }

    // This method can be called to return to the main menu scene
    private void OnMainMenuClicked()
    {
        level.MainMenu();// Call the MainMenu method in the LevelManager to return to the main menu scene
    }

    // This method can be called to exit the game when the exit button is clicked
    private void OnExitClicked()
    {
        level.Exit();// Call the Exit method in the LevelManager to quit the game
    }

    private IEnumerator GameOverSequence()
    {
        gameOverSequenceStarted = true;// Set the flag to indicate that the game over sequence has started to prevent multiple triggers

        yield return new WaitForSeconds(gameOverDelay);// Wait for the specified delay time before showing the game over menu

        level.Pause();// Pause the game by calling the Pause method in the LevelManager to freeze the game

        ShowGameOverMenu();// Show the game over menu after the delay
    }
}
