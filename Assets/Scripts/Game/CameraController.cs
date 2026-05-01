using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] private Transform target;// The target Transform that the camera will follow, can be set in the Inspector
    [SerializeField] private Vector3 offset = new Vector3(0f, 0f, -10f);// The offset from the target's position to maintain, can be set in the Inspector
    [SerializeField] private float smoothSpeed = 5.0f;// The speed at which the camera will smoothly follow the target, can be set in the Inspector
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // LateUpdate is called after all Update functions have been called, used here to ensure the camera follows the target after it has moved in Update
    private void LateUpdate()
    {
        if (target != null)
        {
            Vector3 desiredPosition = target.position + offset;// Calculate the desired position of the camera based on the target's position and the offset
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);// Smoothly interpolate between the current camera position and the desired position using Lerp for a smooth following effect
            transform.position = smoothedPosition;// Update the camera's position to the smoothed position
        }
    }
}
