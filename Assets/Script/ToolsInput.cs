using System;
using Photon.Pun;
using UnityEngine;

public class ToolsInput : MonoBehaviour
{
    public int toolsDamage = 1;
    public bool canAttack;
    public string ResourceType;
    
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
        if (other.gameObject.CompareTag(ResourceType))
        {
            canAttack = true;
            if (canAttack && _getAttack)
            {
                ResourceMaterial resource = other.gameObject.GetComponent<ResourceMaterial>();
                if (resource != null)
                {
                    resource.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.All, toolsDamage);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag(ResourceType))
        {
            canAttack = false;
        }
    }
}
