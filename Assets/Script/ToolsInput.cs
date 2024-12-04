using System;
using Photon.Pun;
using UnityEngine;

public class ToolsInput : MonoBehaviour
{
    public int toolsDamage = 1;
    public bool canAttack;
    public string ResourceType;
    public AudioSource toolsSound;
    
    [SerializeField] bool _getAttack;

    private void Update()
    {
        if (PlayerControl.Instance != null)
        {
            _getAttack = PlayerControl.Instance.isAttacking;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(ResourceType))
        {
            canAttack = true;
            if (canAttack && _getAttack)
            {
                toolsSound.Play();
                ResourceMaterial resource = other.gameObject.GetComponent<ResourceMaterial>();
                if (resource != null)
                {
                    resource.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.All, toolsDamage);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag(ResourceType))
        {
            canAttack = false;
        }
    }
    
    [PunRPC]
    public void GrabTool(int playerViewID)
    {
        // Find the player's PhotonView by its ID
        PhotonView playerPhotonView = PhotonView.Find(playerViewID);

        if (playerPhotonView != null)
        {
            Transform playerHand = playerPhotonView.GetComponent<PlayerControl>().handPosition;

            // Attach the tool to the player's hand
            transform.SetParent(playerHand);
            transform.localPosition = Vector3.zero; // Adjust position as needed
            transform.localRotation = Quaternion.identity; // Reset rotation
        }
    }

    [PunRPC]
    public void DropTool()
    {
        // Detach the tool from the player
        transform.SetParent(null);
    }
}
