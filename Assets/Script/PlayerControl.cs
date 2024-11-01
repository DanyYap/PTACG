using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;

public class PlayerControl : MonoBehaviourPunCallbacks
{
    public static PlayerControl Instance { get; private set; }
    public Animator PlayerAnimation;
    public Transform handPosition;

    public float Speed;
    public float attackCooldown = 1f;
    public bool isAttacking = false;
    
    public bool isHoldingObject = false;
    private bool canAttack = true;
    private float _currentSpeed;
    
    public GameObject objectInRange;
    private Vector2 move;
    private Vector2 input;
    private Vector3 _towardDirection;

    public void OnMove(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>();
    }

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
    
    // Update is called once per frame
    void Update()
    {
        MovePlayer();

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
        Vector3 movement = new Vector3(move.x, 0, move.y);
        if (movement.magnitude > 0.1f)
        {
            PlayerAnimation.SetBool("isRunning", true);
            
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movement), 0.15f);
            
            transform.Translate(movement * Speed * Time.deltaTime, Space.World);
        }
        else
        {
            PlayerAnimation.SetBool("isRunning", false);
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
