using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    [SerializeField] private Transform guide;// The guide Transform that the background will follow, can be set in the Inspector
    [SerializeField] private Vector3 offset = new Vector3(0f, 0f, 10f);// The offset from the guide's position to maintain, can be set in the Inspector
    [SerializeField] private float smoothing = 5f;// The smoothing factor for the background movement, can be set in the Inspector
    public static BackgroundController instance;// Static instance of the BackgroundController for easy access from other scripts, set in Awake()

    void Awake()
    {
        // Set up the singleton pattern for BackgroundController to ensure only one instance exists and can be easily accessed from other scripts
        if (instance == null)
        {
            instance = this;// Set the static instance to this instance of BackgroundController for easy access from other scripts
        }
        else
        {
            Destroy(gameObject);// If an instance already exists, destroy this duplicate to enforce the singleton pattern
        }
    }

    void LateUpdate()
    {
        if (guide != null)
        {
            Vector3 desiredPosition = guide.position + offset;// Calculate the desired position of the background based on the guide's position and the offset
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothing * Time.deltaTime);// Smoothly interpolate between the current background position and the desired position using Lerp for a smooth following effect
            transform.position = smoothedPosition;// Update the background's position to the smoothed position
        }
    }
}
