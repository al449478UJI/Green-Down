using UnityEngine;
using UnityEngine.UIElements;

public class ScoreManager : MonoBehaviour
{
    [Header("Score Settings")]
    [SerializeField] private string enemyTag = "Enemy";// Tag used to identify enemy GameObjects, can be set in the Inspector

    private int enemiesRemaining;// Counter for the number of enemies remaining in the level, initialized in Start() by counting GameObjects with the specified enemy tag
    private bool levelCompleted = false;// bool to track if the level has been completed, used to prevent multiple triggers of level completion logic
    private Label scoreLabel;// Reference to the UI Label for displaying the score, can be set in Start() by querying the UIDocument's root visual element

    [Header("Utility")]
    [SerializeField] private UIDocument uiDocument;// Reference to the UIDocument component for updating the UI, can be set in the Inspector

    public static ScoreManager Instance;// Static instance of the ScoreManager for easy access from other scripts, set in Awake()

    // Awake is called when the script instance is being loaded
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;// Set the static Instance to this instance of ScoreManager, allowing other scripts to access it via ScoreManager.Instance
        }
        else
        {
            Destroy(gameObject);// If an instance already exists, destroy this duplicate to enforce the singleton pattern
        }

        if(uiDocument == null)
        {
            uiDocument = GetComponent<UIDocument>();// Get the UIDocument component attached to the same GameObject if it hasn't been set in the Inspector
        }
    }

    // OnEnable is called when the object becomes enabled and active
    private void OnEnable()
    {
        scoreLabel = uiDocument.rootVisualElement.Q<Label>("Score");// Query the root visual element of the UIDocument for a Label with the name "Score" and store a reference to it for updating the score display
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemiesRemaining = GameObject.FindGameObjectsWithTag(enemyTag).Length;// Count the number of GameObjects in the scene with the specified enemy tag and store it in enemiesRemaining to track how many enemies are left to defeat

        UpdateScoreText();// Call the method to update the score text in the UI based on the initial count of enemies
    }

    // Method to update the score text in the UI based on the current count of enemies remaining
    public void EnemyDefeated()
    {
        if (levelCompleted)
        {
            return;// If the level has already been completed, ignore further calls to this method to prevent multiple triggers of level completion logic
        }
        
        enemiesRemaining--;// Decrement the count of enemies remaining when an enemy is defeated

        UpdateScoreText();// Update the score text in the UI to reflect the new count of enemies

        if (enemiesRemaining <= 0)
        {
            LevelCompleted();// If there are no enemies remaining, call the method to handle level completion logic
        }
    }

    // Method to update the score text in the UI to display the current count of enemies remaining
    private void UpdateScoreText()
    {
        // Check if the scoreLabel reference is valid before trying to update its text to avoid null reference errors
        if (scoreLabel != null)
        {
            scoreLabel.text = "Enemies: " + enemiesRemaining;// Update the text of the scoreLabel to display the current count of enemies remaining
        }
    }

    // Method to handle logic when the level is completed (i.e., all enemies are defeated)
    private void LevelCompleted()
    {
        levelCompleted = true;// Set the levelCompleted flag to true to prevent further triggers of this method

        Debug.Log("Level Completed!");// Log a message to the console indicating that the level has been completed, can be replaced with more complex logic such as transitioning to a new scene or displaying a victory screen
    }
}
