using System;
using UnityEngine;
using Photon.Pun;

public class PlayerControl : MonoBehaviour
{
    public static PlayerControl Instance { get; private set; }
    public Transform handPosition;
    
    public float RotationSpeed = 100f;
    public float attackCooldown = 1f;
    public bool isAttacking = false;

    public float speed = 4f;
    public float maxVelocity = 10f;
    public bool isHoldingObject = false;
    private bool canAttack = true;

    public Animator toolsAnimation;
    public GameObject objectInRange;
    private float rotateInput;
    private Vector2 input;
    private Rigidbody rb;
    public PhotonView photonView;
    public GameObject playerCamera; // Reference to the camera attached to the player prefab
    public MonoBehaviour[] controlScripts;


    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
        if (photonView.IsMine)
        {
            Instance = this;
        }
    }
    
    private void OnDestroy()
    {
        if (photonView.IsMine)
        {
            Instance = null;
        }
    }
    
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        rb.AddForce(CalculatedMovement(speed), ForceMode.VelocityChange);
        if (photonView.IsMine)
        {
            // Activate the camera for the local player
            playerCamera.SetActive(true);

            // Enable input/movement scripts for the local player
            foreach (var script in controlScripts)
            {
                script.enabled = true;
            }
        }
        else
        {
            // Disable the camera for remote players
            playerCamera.SetActive(false);

            // Disable input/movement scripts for remote players
            foreach (var script in controlScripts)
            {
                script.enabled = false;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        RotatePlayer();
        
        input = new Vector2(0, Input.GetAxisRaw("Vertical"));
        input.Normalize();
        
        if (Input.GetMouseButtonDown(0) && canAttack)
        {
            Attack();
            isAttacking = true;
            toolsAnimation.SetBool("isAttack", true);
        }
        
        if (objectInRange != null && Input.GetKeyDown(KeyCode.E) && !isHoldingObject)
        {
            PickUpObject();
        }
        else if (isHoldingObject && Input.GetKeyDown(KeyCode.E))
        {
            DropObject();
        }
    }

    Vector3 CalculatedMovement(float _speed)
    {
        Vector3 targetVelocity = new Vector3(input.x, 0, input.y);
        targetVelocity = transform.TransformDirection(targetVelocity);

        targetVelocity *= _speed;

        Vector3 velocity = rb.velocity;

        if (input.magnitude > 0.5f)
        {
            Vector3 velocityChange = targetVelocity - velocity;

            velocityChange.x = Math.Clamp(velocityChange.x, -maxVelocity, maxVelocity);
            velocityChange.z = Math.Clamp(velocityChange.z, -maxVelocity, maxVelocity);

            velocityChange.y = 0;

            return (velocityChange);
        }
        else
        {
            return new Vector3();
        }
    }
    
    
    public void RotatePlayer()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        rotateInput = horizontalInput;
        if (rotateInput != 0)
        {
            // Rotate player based on horizontal input
            transform.Rotate(Vector3.up * rotateInput * RotationSpeed * Time.deltaTime);
        }
    }
    
    void Attack()
    {
        Debug.Log("Player Attacked!");
        
        canAttack = false;
        Invoke("ResetAttack", attackCooldown);
    }

    void ResetAttack()
    {
        isAttacking = false;
        canAttack = true;
    }
    
    public void PickUpObject()
    {
        if (objectInRange != null)
        {
            if (objectInRange.CompareTag("Interactable")) // If it's an interactable object
            {
                objectInRange.transform.SetParent(handPosition); // Attach to hand
            }
            
            objectInRange.transform.localPosition = Vector3.zero; // Position at parent
            objectInRange.transform.localRotation = Quaternion.identity; // Reset rotation
            isHoldingObject = true;
        }
    }

    public void DropObject()
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
        if (!isHoldingObject && other.CompareTag("Interactable"))
        {
            objectInRange = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!isHoldingObject && objectInRange == other.gameObject)
        {
            objectInRange = null;
        }
    }
}
