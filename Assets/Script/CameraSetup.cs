using UnityEngine;
using Photon.Pun;

public class CameraSetup : MonoBehaviour
{
    public Transform target;
    public float smoothTime = 0.3f;
    public Vector3 offSet;

    private Vector3 velocity = Vector3.zero;

    void Start()
    {
        if (PhotonView.Get(this).IsMine)
        {

        }
    }

    private void Update()
    {
        if (target != null)
        {
            Vector3 targetPosition = target.position + offSet;
            
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        }
    }
}
