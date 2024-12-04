using UnityEngine;
using Photon.Pun;
using UnityEngine.InputSystem;

public class PushableObject : MonoBehaviourPun
{
    private Rigidbody rb;
    public float pushForce = 10f;
    public float interactionDistance = 2f; // How close the player needs to be to interact
    public GameObject materialObject;
    private PlayerControl player;  // Reference to player control script
    private bool isInteracting = false;

    void Start()
    {
        rb = materialObject.GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Check for input (example: E key)
        if (isInteracting && Input.GetKeyDown(KeyCode.E))
        {
            // Call interaction function when the player presses E
            InteractWithObject();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.GetComponent<PlayerControl>();
            isInteracting = true;  // Player is in range to interact
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = null;
            isInteracting = false;  // Player left range
        }
    }

    public void InteractWithObject()
    {
        // Only allow interaction if the object is not already owned by another player
        if (photonView.IsMine == false)
        {
            photonView.TransferOwnership(PhotonNetwork.LocalPlayer);  // Transfer ownership
        }

        // Apply force or interaction logic
        if (photonView.IsMine)
        {
            // Example of applying force when interacting with the object (pushing it)
            Vector3 pushDirection = player.transform.forward;  // Direction of the push (forward of the player)
            rb.AddForce(pushDirection * pushForce, ForceMode.VelocityChange);
            photonView.RPC("SyncPush", RpcTarget.OthersBuffered, pushDirection);  // Sync push action with others
        }
    }

    [PunRPC]
    private void SyncPush(Vector3 direction)
    {
        if (!photonView.IsMine) // Only apply the force on other clients
        {
            rb.AddForce(direction * pushForce, ForceMode.VelocityChange);
        }
    }
}
