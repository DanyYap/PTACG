using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Animator axeAnimator;
    public float walkSpeed = 4f;
    public float maximumSpeed = 10f;
    public float attackCooldown = 1f;
    public bool isAttacking = false;

    private bool canAttack = true;
    
    private Rigidbody rb;
    private Vector2 input;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        input.Normalize();

        if (Input.GetMouseButtonDown(0) && canAttack)
        {
            Attack();
            axeAnimator.SetTrigger("isAttack");
        }
    }

    void FixedUpdate()
    {
        rb.AddForce(CalculateMovement(walkSpeed),ForceMode.VelocityChange);
    }
    

    Vector3 CalculateMovement(float _speed)
    {
        Vector3 targetVelocity = new Vector3(input.x, 0, input.y);
        targetVelocity = transform.TransformDirection(targetVelocity);

        targetVelocity *= _speed;
        Vector3 velocity = rb.velocity;

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
}
