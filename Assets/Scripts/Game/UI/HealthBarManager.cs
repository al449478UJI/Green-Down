using UnityEngine;
using UnityEngine.UIElements;

public class HealthBarManager : MonoBehaviour
{
    [Header("Health Bar Utilities")]
    [SerializeField] private UIDocument UIDocument;
    [SerializeField] private Sprite[] healthSprites;

    private Image healthBarImage;
    public static HealthBarManager instance;

    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        // Set up the singleton pattern for HealthBarManager to ensure only one instance exists and can be easily accessed from other scripts
        if (instance == null)
        {
            instance = this;// Set the static instance to this instance of HealthBarManager for easy access from other scripts
        }
        else
        {
            Destroy(gameObject);// If an instance already exists, destroy this duplicate to enforce the singleton pattern
        }

        if (UIDocument == null)
        {
            UIDocument = GetComponent<UIDocument>();
        }
    }

    // OnEnable is called when the object becomes enabled and active
    void OnEnable()
    {
        healthBarImage = UIDocument.rootVisualElement.Q<Image>("HealthBar");
    }

    // Method to update the health bar based on the current health
    public void UpdateHealthBard(int currentHealth)
    {
        currentHealth = Mathf.Clamp(currentHealth, 0, healthSprites.Length - 1); // Ensure currentHealth is within the valid range of healthSprites

        // Update the health bar image if it exists
        if (healthBarImage != null)
        {
            healthBarImage.sprite = healthSprites[currentHealth];// Set the sprite based on the current health
        }
    }
}
