using System;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public static PlayerControl Instance { get; private set; }
    public Animator PlayerAnimation;
    public Transform handPosition;
    
    public float walkSpeed = 4f;
    public float maximumSpeed = 10f;
    public float attackCooldown = 1f;
    public bool isAttacking = false;
    
    private bool isHoldingObject = false;
    private bool canAttack = true;
    private float _currentSpeed;
    
    private GameObject objectInRange;
    private Rigidbody rb;
    private Vector2 input;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
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
        input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        input.Normalize();

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
        
        PlayerAnimation.SetFloat("Speed", _currentSpeed);
    }

    void FixedUpdate()
    {
        rb.AddForce(CalculateMovement(walkSpeed),ForceMode.VelocityChange);
    }
    
    void RotatePlayer(float horizontalInput)
    {
        // Determine the target direction based on horizontal input
        Vector3 targetDirection = new Vector3(horizontalInput, 0f, 0f).normalized;

        if (targetDirection != Vector3.zero)
        {
            // Calculate the rotation towards the target direction
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

            // Smoothly rotate the player towards the target direction
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
    

    Vector3 CalculateMovement(float _speed)
    {
        Vector3 targetVelocity = new Vector3(input.x, 0, input.y);
        targetVelocity = transform.TransformDirection(targetVelocity);

        targetVelocity *= _speed;
        Vector3 velocity = rb.velocity;
        _currentSpeed = input.magnitude;

        if (input.magnitude > 0.5f)
        {
            Vector3 velocityChange = targetVelocity - velocity;

            velocityChange.x = Mathf.Clamp(velocityChange.x, -maximumSpeed, maximumSpeed);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -maximumSpeed, maximumSpeed);

            velocityChange.y = 0;

            return (velocityChange);
        }
        else
        {
            return new Vector3();
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
    
    void PickUpObject()
    {
        if (objectInRange != null)
        {
            if (objectInRange.CompareTag("Tools")) // Check if it's a tool
            {
                objectInRange.transform.SetParent(handPosition); // Attach to pivot
            }
            else if (objectInRange.CompareTag("Interactable")) // If it's an interactable object
            {
                objectInRange.transform.SetParent(handPosition); // Attach to hand
            }
            
            objectInRange.transform.localPosition = Vector3.zero; // Position at parent
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
        if (other.CompareTag("Interactable") || other.CompareTag("Tools"))
        {
            objectInRange = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Interactable") || other.CompareTag("Tools"))
        {
            objectInRange = null;
        }
    }
}
