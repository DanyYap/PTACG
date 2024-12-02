using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;

public class MaterialProduce : MonoBehaviourPunCallbacks
{
    public float productionTime = 5f; // Time to produce material
    public GameObject producedMaterialPrefab; // Material to be produced
    public Transform outputPoint; // Where the material will appear
    public string ResourceLayerType;
    public AudioSource produceSound;
    
    private InputAction interactAction;
    private bool isProcessing = false;
    [SerializeField] PlayerControl playerInRange = null;
    
    private void Update()
    {
        // Check for interaction input
        if (playerInRange != null && Input.GetKeyDown(KeyCode.E))
        {
            OnInteract();
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (!isProcessing && other.CompareTag("Player"))
        {
            // Player entered range
            playerInRange = other.GetComponent<PlayerControl>();
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (playerInRange != null && other.CompareTag("Player"))
        {
            // Player left range
            playerInRange = null;
        }
    }
    
    private void OnInteract()
    {
        // Check if the player is in range and holding the correct resource
        if (playerInRange != null && playerInRange.isHoldingObject && playerInRange.objectInRange.layer == LayerMask.NameToLayer(ResourceLayerType))
        {
            // Start the production process if not already processing
            if (!isProcessing)
            {
                photonView.RPC("StartProduction", RpcTarget.All, playerInRange.photonView.ViewID);
            }
        }
    }
    
    [PunRPC]
    private void StartProduction(int playerViewID)
    {
        if (!isProcessing)
        {
            PlayerControl player = PhotonView.Find(playerViewID).GetComponent<PlayerControl>();
            if (player != null)
            {
                StartCoroutine(ProduceMaterial(player));
                Destroy(player.objectInRange);
            }
        }
    }

    private IEnumerator ProduceMaterial(PlayerControl player)
    {
        isProcessing = true;
        
        if (produceSound != null)
        {
            produceSound.Play();
        }
        
        // Play production animation or feedback
        Debug.Log("Production started...");

        // Simulate production delay
        yield return new WaitForSeconds(productionTime);

        // Instantiate the material at the output point across the network
        PhotonNetwork.Instantiate(producedMaterialPrefab.name, outputPoint.position, outputPoint.rotation);
        Debug.Log("Material produced!");
        
        if (produceSound != null && produceSound.isPlaying)
        {
            produceSound.Stop();
        }

        isProcessing = false;
    }
}
