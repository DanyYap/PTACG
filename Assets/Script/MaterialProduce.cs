using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class MaterialProduce : MonoBehaviour
{
    public float productionTime = 5f; // Time to produce material
    public GameObject producedMaterialPrefab; // Material to be produced
    public Transform outputPoint; // Where the material will appear

    private InputAction interactAction;
    private bool isProcessing = false;
    private PlayerControl playerInRange = null;

    
    private void Awake()
    {
        interactAction = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/e");
    }
    
    private void OnEnable()
        {
            // Enable the input action for interacting with the machine
            interactAction.Enable();
            interactAction.performed += OnInteract;
        }
    
    private void OnDisable()
    {
        interactAction.performed -= OnInteract;
        interactAction.Disable();
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
    
    private void OnInteract(InputAction.CallbackContext context)
    {
        // Check if the player is in range and holding the correct resource
        if (playerInRange != null && playerInRange.isHoldingObject && playerInRange.objectInRange.layer == LayerMask.NameToLayer("Resource"))
        {
            // Start the production process if not already processing
            if (!isProcessing)
            {
                StartCoroutine(ProduceMaterial(playerInRange));
                Destroy(playerInRange.objectInRange);
            }
        }
    }

    private IEnumerator ProduceMaterial(PlayerControl player)
    {
        isProcessing = true;

        // Play production animation or feedback
        Debug.Log("Production started...");

        // Simulate production delay
        yield return new WaitForSeconds(productionTime);

        // Instantiate the material at the output point
        Instantiate(producedMaterialPrefab, outputPoint.position, outputPoint.rotation);
        Debug.Log("Material produced!");

        // Destroy or deactivate the resource object that the player was holding
        Destroy(player.objectInRange); // Or use player.objectInRange.SetActive(false);

        // Clear player's holding object after production
        player.DropObject();

        isProcessing = false;
    }
}
