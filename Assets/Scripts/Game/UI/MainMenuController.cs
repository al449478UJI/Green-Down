using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuController : MonoBehaviour
{
    private VisualElement mainMenu;// Reference to the main menu UI element
    private Button startButton;// Reference to the start button in the main menu
    private Button controlsButton;// Reference to the controls button in the main menu
    private Button exitButton;// Reference to the exit button in the main menu

    [Header("Utilities")]
    [SerializeField] private UIDocument uiDocument;// Reference to the UIDocument component that contains the pause menu UI
    [SerializeField]private MainMenuManager mainMenuManager;

    private void Awake()
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
        mainMenu = root.Q<VisualElement>("MainMenu");// Find the main menu element by name
        startButton = root.Q<Button>("StartButton");// Find the start button by name
        controlsButton = root.Q<Button>("ControlsButton");// Find the controls button by name
        exitButton = root.Q<Button>("ExitButtonMm");// Find the exit button by name

        startButton.clicked += StartGame;// Add a click event listener to the start button
        controlsButton.clicked += ShowControls;// Add a click event listener to the controls button
        exitButton.clicked += ExitGame;// Add a click event listener to the exit button
    }

    // OnDisable is called when the behaviour becomes disabled or inactive
    private void OnDisable()
    {
        // Remove the click event listeners when the object is disabled to prevent memory leaks
        startButton.clicked -= StartGame;// Remove the click event listener from the start button
        controlsButton.clicked -= ShowControls;// Remove the click event listener from the controls button
        exitButton.clicked -= ExitGame;// Remove the click event listener from the exit button
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void StartGame()
    {
        mainMenuManager.StartG();// Call the StartG method in the MainMenuManager to start the game
    }

    private void ShowControls()
    {
        // Implement the logic to show the controls menu
    }

    private void ExitGame()
    {
        mainMenuManager.Exit();// Call the Exit method in the MainMenuManager to exit the game
    }

}