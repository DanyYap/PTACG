using System;
using Photon.Pun;
using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    public bool isLocalPlayer;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PhotonView photonView = other.GetComponent<PhotonView>();

            // Check if the other object has a PhotonView and if it is the local player
            if (photonView != null && photonView.IsMine)
            {
                // Destroy the player object locally
                PhotonNetwork.Destroy(other.gameObject);

                // Call the RespawnPlayer function from RoomManager
                RoomManager.instance.RespawnPlayer();
            }
        }
    }
}

