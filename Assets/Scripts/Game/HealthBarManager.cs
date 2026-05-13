using UnityEngine;
using UnityEngine.UIElements;

public class HealthBarManager : MonoBehaviour
{
    [Header("Health Bar Utilities")]
    [SerializeField] private UIDocument UIDocument;
    [SerializeField] private Sprite[] healthSprites;
    private Image healthBarImage;

    // Awake is called when the script instance is being loaded
    private void Awake()
    {
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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
