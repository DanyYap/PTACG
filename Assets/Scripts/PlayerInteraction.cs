using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInteraction : MonoBehaviour
{
    public Transform handPosition;      // The position where the object will be held
    public TextMeshProUGUI interactText; // UI Text displaying "Press E to pick up"

    private GameObject currentInteractable;  // Object player can interact with
    private GameObject objectInHand;         // Object player is holding

    void Update()
    {
        if (currentInteractable != null && objectInHand == null)
        {
            interactText.text = "Press E to pick up";
            interactText.gameObject.SetActive(true);

            if (Input.GetKeyDown(KeyCode.E))
            {
                // Pick up the object
                PickUpObject();
            }
        }
        else if (objectInHand != null)
        {
            interactText.text = "Press E to drop";

            if (Input.GetKeyDown(KeyCode.E))
            {
                // Drop the object
                DropObject();
            }
        }
        else
        {
            interactText.gameObject.SetActive(false);
        }
    }


    void PickUpObject()
    {
        objectInHand = currentInteractable;
        objectInHand.transform.SetParent(handPosition); // Parent to the hand position

        // Adjust local position to ensure it is placed correctly
        objectInHand.transform.localPosition = new Vector3(0f, 0f, 0.5f); // Adjust this for proper placement in front of the player
        objectInHand.transform.localRotation = Quaternion.Euler(0f, -30f, 0f); // Set rotation to -30 degrees on the Y-axis

        // Disable the collider to prevent collisions while holding the torch
        Collider objectCollider = objectInHand.GetComponent<Collider>();
        if (objectCollider != null)
        {
            objectCollider.enabled = false; // Disable collider to prevent collisions while holding
        }

        // Disable the Rigidbody to prevent physics interactions while holding
        Rigidbody rb = objectInHand.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true; // Disable physics interactions while held
        }

        interactText.text = "Press E to drop"; // Change text for dropping
    }


    void DropObject()
    {
        // Unparent the object
        objectInHand.transform.SetParent(null);

        Rigidbody rb = objectInHand.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false; // Enable physics
            rb.AddForce(Camera.main.transform.forward * 5f, ForceMode.Impulse); // Add a slight forward force for a realistic drop
        }

        objectInHand.GetComponent<Collider>().enabled = true; // Re-enable collider
        objectInHand = null; // Clear reference
        interactText.text = "Press E to pick up"; // Reset text to pick up
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Interactable"))
        {
            currentInteractable = other.gameObject; // Store the interactable object
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == currentInteractable)
        {
            currentInteractable = null; // Clear reference when the player leaves the area
            interactText.gameObject.SetActive(false); // Hide the prompt when out of range
        }
    }
}
