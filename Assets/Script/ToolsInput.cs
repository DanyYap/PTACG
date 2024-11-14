using Photon.Pun;
using UnityEngine;

public class ToolsInput : MonoBehaviour
{
    public int toolsDamage = 1;
    
    [SerializeField] bool _getAttack;

    private void Update()
    {
        if (PlayerControl.Instance != null)
        {
            _getAttack = PlayerControl.Instance.isAttacking;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Resource") && _getAttack)
        {
            ResourceMaterial resource = other.gameObject.GetComponent<ResourceMaterial>();
            if (resource != null)
            {
                resource.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.All, toolsDamage);
            }
        }
    }
}
