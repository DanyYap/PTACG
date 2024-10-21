using UnityEngine;
using Cinemachine;
using Photon.Pun;

public class CameraSetup : MonoBehaviour
{
    public CinemachineVirtualCamera cinemachineCam;  // Reference to the Cinemachine Virtual Camera

    void Start()
    {
        if (PhotonView.Get(this).IsMine)
        {
            cinemachineCam.Follow = transform;
            cinemachineCam.LookAt = transform;
        }
    }
}
