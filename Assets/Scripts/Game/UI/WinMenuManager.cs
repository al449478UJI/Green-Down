using UnityEngine;
using UnityEngine.UIElements;

public class WinMenuManager : MonoBehaviour
{
    private VisualElement winMenu;// Reference to the win menu UI element
    private Button restartButton;// Reference to the restart button in the win menu
    private Button exitButton;// Reference to the exit button in the win menu
    private Button mainMenuButton;// Reference to the main menu button in the win menu
    private bool win = false;// Prevents starting the win sequence more than once

    [Header("Utilities")]
    [SerializeField] private UIDocument uiDocument;// Reference to the UIDocument component that contains the win menu UI
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

        winMenu = root.Q<VisualElement>("WinMenu");// Find the win menu element by name
        restartButton = root.Q<Button>("RestartButtonW");// Find the restart button by name
        exitButton = root.Q<Button>("ExitButtonW");// Find the exit button by name
        mainMenuButton = root.Q<Button>("MainMenuButtonW");// Find the main menu button by name

        restartButton.clicked += OnRestartClicked;// Add a click event listener to the restart button
        exitButton.clicked += OnExitClicked;// Add a click event listener to the exit button
        mainMenuButton.clicked += OnMainMenuClicked;// Add a click event listener to the main menu button

        winMenu.style.display = DisplayStyle.None;// Initially hide the win menu when the game starts
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
        // Check if the level is completed and the win sequence hasn't already started to prevent multiple triggers of the win sequence
        if (ScoreManager.Instance.levelCompleted && !win)
        {
            win = true;// Set the win flag to true to prevent multiple triggers of the win sequence

            this.ShowWinMenu();// Show the win menu when the player wins

            level.Pause();// Pause the game when the player wins
        }

        // Check if the level is not completed and the win sequence has already started, which means the player has restarted or returned to the main menu from the win menu
        else if (!ScoreManager.Instance.levelCompleted && win)
        {
            win = false;// Reset the win flag to false when the level is not completed, allowing the win sequence to be triggered again if the player restarts

            this.HideWinMenu();// Hide the win menu when the player restarts or exits

            level.Resume();// Resume the game when the player restarts or exits
        }
    }

    // This method can be called to show the win menu when the player wins
    private void ShowWinMenu()
    {
        winMenu.style.display = DisplayStyle.Flex;// Show the win menu when the player wins
    }

    // This method can be called to hide the win menu when the player restarts or exits
    private void HideWinMenu()
    {
        winMenu.style.display = DisplayStyle.None;// Hide the win menu when the player restarts or exits
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
}
