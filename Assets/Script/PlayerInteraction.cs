using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public Transform handPosition;      // The position where the object will be held
    
    private GameObject objectInRange;
    private bool isHoldingObject = false;


    void Update()
    {
        if (objectInRange != null && Input.GetKeyDown(KeyCode.E) && !isHoldingObject)
        {
            PickUpObject();
        }
        else if (isHoldingObject && Input.GetKeyDown(KeyCode.E))
        {
            DropObject();
        }
    }


    void PickUpObject()
    {
        if (objectInRange != null)
        {
            objectInRange.transform.SetParent(handPosition); // Attach object to hand
            objectInRange.transform.localPosition = Vector3.zero; // Position it at the hand position
            objectInRange.transform.localRotation = Quaternion.identity; // Reset rotation
            isHoldingObject = true;
        }
    }


    void DropObject()
    {
        if (isHoldingObject)
        {
            objectInRange.transform.SetParent(null);
            objectInRange = null;
            isHoldingObject = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Interactable"))
        {
            objectInRange = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Interactable"))
        {
            objectInRange = null;
        }
    }
}
