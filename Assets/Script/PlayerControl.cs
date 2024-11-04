using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;

public class PlayerControl : MonoBehaviourPunCallbacks
{
    public static PlayerControl Instance { get; private set; }
    public Animator PlayerAnimation;
    public Transform handPosition;

    public float Speed;
    public float RotationSpeed = 100f;
    public float attackCooldown = 1f;
    public bool isAttacking = false;
    
    public bool isHoldingObject = false;
    private bool canAttack = true;
    private float _currentSpeed;
    
    public GameObject objectInRange;
    private float moveInput;
    private float rotateInput;
    private Vector2 move;
    private Vector2 input;
    private Vector3 _towardDirection;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void OnMove(InputAction.CallbackContext context)
    {
        // Read vertical input for forward and backward movement
        moveInput = context.ReadValue<Vector2>().y;
    }

    public void OnRotate(InputAction.CallbackContext context)
    {
        // Read horizontal input for rotation
        rotateInput = context.ReadValue<Vector2>().x;
    }
    
    // Update is called once per frame
    void Update()
    {
        MovePlayer();
        RotatePlayer();

        if (Input.GetMouseButtonDown(0) && canAttack)
        {
            Attack();
            PlayerAnimation.SetTrigger("isAttack");
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

    public void MovePlayer()
    {
        Vector3 forwardMovement = transform.forward * moveInput;
        
        if (moveInput != 0)
        {
            PlayerAnimation.SetBool("isRunning", true);
            transform.Translate(forwardMovement * Speed * Time.deltaTime, Space.World);
        }
        else
        {
            PlayerAnimation.SetBool("isRunning", false);
        }
    }
    
    public void RotatePlayer()
    {
        if (rotateInput != 0)
        {
            // Rotate player based on horizontal input
            transform.Rotate(Vector3.up * rotateInput * RotationSpeed * Time.deltaTime);
        }
    }
    
    void Attack()
    {
        Debug.Log("Player Attacked!");
        isAttacking = true;
        
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
