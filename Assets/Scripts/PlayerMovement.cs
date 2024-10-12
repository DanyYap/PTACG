using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public Camera playerCamera; // The player's camera
    public Transform handPosition; // Empty GameObject in front of the camera for holding the torch
    public float walkSpeed = 6f;
    public float runSpeed = 12f;
    public float jumpPower = 7f;
    public float gravity = 10f;
    public float lookSpeed = 2f;
    public float lookXLimit = 45f;

    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0;
    private CharacterController characterController;

    private bool canMove = true;
    private GameObject torchInHand = null; // Reference to the torch when picked up
    private GameObject currentInteractable; // Object player can interact with

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Handle player movement
        HandleMovement();

        // Handle Torch Pickup/Drop Logic
        HandleTorchPickupAndDrop();
    }

    void HandleMovement()
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float curSpeedX = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpPower;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        characterController.Move(moveDirection * Time.deltaTime);

        // Handle camera movement
        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
    }

    void HandleTorchPickupAndDrop()
    {
        if (torchInHand == null && currentInteractable != null && Input.GetKeyDown(KeyCode.E))
        {
            // Pick up the torch
            torchInHand = currentInteractable;
            torchInHand.transform.SetParent(handPosition); // Attach to the hand position (in front of the camera)

            // Adjust torch position and rotation to simulate holding it properly
            torchInHand.transform.localPosition = new Vector3(0.3f, -0.2f, 0.5f); // Adjust this for better positioning
            torchInHand.transform.localRotation = Quaternion.Euler(0, -90, 0); // Adjust rotation as needed
        }
        else if (torchInHand != null && Input.GetKeyDown(KeyCode.E))
        {
            // Drop the torch
            torchInHand.transform.SetParent(null); // Unparent it so it "drops"
            torchInHand = null; // Clear reference
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Interactable"))
        {
            currentInteractable = other.gameObject; // Reference the interactable object (torch)
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == currentInteractable)
        {
            currentInteractable = null;
        }
    }
}
