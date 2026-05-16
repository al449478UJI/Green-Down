using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenuManager : MonoBehaviour
{
    private VisualElement mainMenuUI;// Reference to the game UI element, which can be used to show or hide the UI when pausing or resuming the game

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
        mainMenuUI = uiDocument.rootVisualElement.Q<VisualElement>("MainMenu");// Get the root visual element of the UIDocument and assign it to the mainMenuUI variable

        mainMenuUI.style.display = DisplayStyle.Flex;// Ensure the main menu UI is visible when the scene starts
    }

    public void StartG()
    {
        Time.timeScale = 1f;// Ensure the time scale is reset to normal before starting the game
        SceneManager.LoadScene("Level");// Load the first level scene when the start button is clicked
    }

    // This method is called when the controls button is clicked in the main menu
    public void Controls()
    {
        Time.timeScale = 1f;// Ensure the time scale is reset to normal before going to the controls menu
        SceneManager.LoadScene("ControlsMenu");// Load the controls menu scene when the controls button is clicked
    }

    public void Exit()
    {
        Time.timeScale = 1f;// Ensure the time scale is reset to normal before exiting the game
        Application.Quit();// Quit the application when the exit button is clicked
        Debug.Log("Quit game");
    }
}
