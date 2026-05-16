using UnityEngine;
using UnityEngine.UIElements;

public class ControlsMenuController : MonoBehaviour
{
    private VisualElement controlsMenu;// Reference to the controls menu UI element
    private Button backButton;// Reference to the back button in the controls menu

    [Header("Utilities")]
    [SerializeField] private UIDocument uiDocument;// Reference to the UIDocument component that contains the pause menu UI
    [SerializeField] private ControlsMenuManager controlsMenuManager;// Reference to the ControlsMenuManager script to call its methods when buttons are clicked

    // Awake is called when the script instance is being loaded
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
        controlsMenu = root.Q<VisualElement>("ControlsMenuUI");// Find the controls menu element by name
        backButton = root.Q<Button>("BackButton");// Find the back button by name

        backButton.clicked += controlsMenuManager.MainMenu;// Add a click event listener to the back button that calls the MainMenu method in the ControlsMenuManager script when clicked
    }

    // OnDisable is called when the behaviour becomes disabled or inactive
    private void OnDisable()
    {
        backButton.clicked -= controlsMenuManager.MainMenu;// Remove the click event listener from the back button when the object is disabled to prevent memory leaks
    }

    private void Back()
    {
        controlsMenuManager.MainMenu();// Call the MainMenu method in the ControlsMenuManager script to go back to the main menu when the back button is clicked
    }
}
