using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Camera Limits")]
    [SerializeField] private float minX;// Minimum X position the camera can move to, can be set in the Inspector
    [SerializeField] private float maxX;// Maximum X position the camera can move to, can be set in the Inspector
    [SerializeField] private float minY;// Minimum Y position the camera can move to, can be set in the Inspector
    [SerializeField] private float maxY;// Maximum Y position the camera can move to, can be set in the Inspector

    [Header("Camera Settings")]
    [SerializeField] private Transform target;// The target Transform that the camera will follow, can be set in the Inspector
    [SerializeField] private Vector3 offset = new Vector3(0f, 0f, -10f);// The offset from the target's position to maintain, can be set in the Inspector
    [SerializeField] private float smoothSpeed = 5.0f;// The speed at which the camera will smoothly follow the target, can be set in the Inspector

    private Camera cam;
    public static CameraController instance;// Static instance of the CameraController for easy access from other scripts, set in Awake()

    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        // Set up the singleton pattern for CameraController to ensure only one instance exists and can be easily accessed from other scripts
        if (instance == null)
        {
            instance = this;// Set the static instance to this instance of CameraController for easy access from other scripts
        }
        else
        {
            Destroy(gameObject);// If an instance already exists, destroy this duplicate to enforce the singleton pattern
        }

        cam = GetComponent<Camera>();// Get the Camera component attached to this GameObject for later use in calculating camera bounds
    }

    // LateUpdate is called after all Update functions have been called, used here to ensure the camera follows the target after it has moved in Update
    private void LateUpdate()
    {
        if (target != null)
        {
            Vector3 desiredPosition = target.position + offset;// Calculate the desired position of the camera based on the target's position and the offset

            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);// Smoothly interpolate between the current camera position and the desired position using Lerp for a smooth following effect

            float cameraHalfHeight = cam.orthographicSize;// Calculate the half height of the camera's view based on its orthographic size

            float cameraHalfWidth = cam.aspect * cameraHalfHeight;// Calculate the half width of the camera's view based on its aspect ratio and half height

            float clampedX = Mathf.Clamp(smoothedPosition.x, minX + cameraHalfWidth, maxX - cameraHalfWidth);// Clamp the X position of the camera to ensure it stays within the defined limits, accounting for the camera's half width

            float clampedY = Mathf.Clamp(smoothedPosition.y, minY + cameraHalfHeight, maxY - cameraHalfHeight);// Clamp the Y position of the camera to ensure it stays within the defined limits, accounting for the camera's half height

            transform.position = new Vector3(clampedX, clampedY, offset.z);// Update the camera's position to the clamped position
        }
    }

    // OnDrawGizmos is called by the Unity Editor to allow you to draw Gizmos in the scene view, used here to visualize the camera limits and current camera view
    private void OnDrawGizmos()
    {
        Camera currentCam = GetComponent<Camera>();

        if (currentCam == null || !currentCam.orthographic)
            return;

        // Draw the world limits
        Gizmos.color = Color.red;
        Vector3 limitsCenter = new Vector3((minX + maxX) / 2f, (minY + maxY) / 2f, 0f);
        Vector3 limitsSize = new Vector3(maxX - minX, maxY - minY, 0f);
        Gizmos.DrawWireCube(limitsCenter, limitsSize);

        // Draw the current camera view
        float cameraHalfHeight = currentCam.orthographicSize;
        float cameraHalfWidth = cameraHalfHeight * currentCam.aspect;

        Gizmos.color = Color.green;
        Vector3 cameraViewCenter = new Vector3(transform.position.x, transform.position.y, 0f);
        Vector3 cameraViewSize = new Vector3(cameraHalfWidth * 2f, cameraHalfHeight * 2f, 0f);
        Gizmos.DrawWireCube(cameraViewCenter, cameraViewSize);
    }
}
