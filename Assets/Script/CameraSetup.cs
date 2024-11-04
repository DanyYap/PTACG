using UnityEngine;
using Photon.Pun;

public class CameraSetup : MonoBehaviour
{
    private Quaternion initialRotation;

    void Start()
    {
        // Store the camera's initial rotation
        initialRotation = transform.localRotation;
    }

    void LateUpdate()
    {
        // Reset the camera's local rotation to its initial rotation
        transform.localRotation = initialRotation;
    }
}
