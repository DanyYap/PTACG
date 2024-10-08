using UnityEngine;

public class ToolsInput : MonoBehaviour
{
    public int toolsDamage = 1;
    public PlayerControl playerScript;
    
    [SerializeField] bool _getAttack;

    private void Update()
    {
        _getAttack = playerScript.isAttacking;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Resource") && _getAttack)
        {
            ResourceMaterial resource = other.gameObject.GetComponent<ResourceMaterial>();
            if (resource != null)
            {
                resource.TakeDamage(toolsDamage);
            }
        }
    }
}
