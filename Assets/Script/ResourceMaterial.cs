using UnityEngine;
using TMPro;

public class ResourceMaterial : MonoBehaviour
{
    public GameObject resourcePrefab;
    public int Health;
    
    

    // Update is called once per frame
    void Update()
    {
        if (Health <= 0)
        {
            DropLog();
            Destroy(gameObject); // Destroy the tree
        }
    }

    public void TakeDamage(int damage)
    {
        Health -= damage;
        Debug.Log("Tree hit! Remaining health: " + Health);
    }

    void DropLog()
    {
        Instantiate(resourcePrefab, transform.position, Quaternion.identity);  // Drop the log at the tree's position
        Debug.Log("Log dropped!");
    }
}
