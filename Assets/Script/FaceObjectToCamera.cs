using UnityEngine;

public class FaceObjectToCamera : MonoBehaviour
{
    private Camera mainCamera;
    
    void Start()
    {
        // Find the main camera in the scene
        mainCamera = Camera.main;
    }
    void Update()
    {
        if (mainCamera != null)
        {
            // Make the player name face the camera
            transform.LookAt(mainCamera.transform);
            
            // Adjust rotation to keep it upright
            transform.rotation = Quaternion.LookRotation(transform.position - mainCamera.transform.position);
        }
    }
}
