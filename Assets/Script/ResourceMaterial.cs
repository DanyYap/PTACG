using Photon.Pun;
using UnityEngine;
using TMPro;

public class ResourceMaterial : MonoBehaviour
{
    public GameObject resourcePrefab;
    public int health;
    
    private Vector3 originalPosition;
    private PhotonView photonView;

    void Start()
    {
        // Save the original position of the tree
        originalPosition = transform.localPosition;photonView = GetComponent<PhotonView>();
        
    }
    
    [PunRPC]
    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log("hit! Remaining health: " + health);
        if (health <= 0)
        {
            if (photonView.IsMine)
            {
                DropLog();
            }
            
            Destroy(gameObject);
            
        }
    }
    
    
    [PunRPC] 
    void DropLog()
    {
        // Use PhotonNetwork.Instantiate to create a networked object visible to all clients
        PhotonNetwork.Instantiate(resourcePrefab.name, transform.position, Quaternion.identity);
        Debug.Log("Log dropped and instantiated for all players!");
    }
}
