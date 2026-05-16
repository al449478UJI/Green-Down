using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class ControlsMenuManager : MonoBehaviour
{
    private VisualElement ControlsMenuUI;// Reference to the game UI element, which can be used to show or hide the UI when pausing or resuming the game

    [Header("Utilities")]
    [SerializeField] private UIDocument uiDocument;// Reference to the UIDocument component that contains the game UI

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
        ControlsMenuUI = uiDocument.rootVisualElement.Q<VisualElement>("ControlsMenu");// Get the root visual element of the UIDocument and find the ControlsMenuUI element by its name

        ControlsMenuUI.style.display = DisplayStyle.Flex;// Ensure the controls menu UI is visible when the scene starts
    }

    // This method is called when the back button is clicked in the controls menu
    public void MainMenu()
    {
        Time.timeScale = 1f;// Ensure the time scale is reset to normal before going back to the main menu
        SceneManager.LoadScene("MainMenu");// Load the main menu scene when the back button is clicked
    }
}
