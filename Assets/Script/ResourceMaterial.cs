using UnityEngine;
using TMPro;

public class ResourceMaterial : MonoBehaviour
{
    public TMP_Text resourceLabel;
    public GameObject logPrefab;
    public int treeHealth;
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        string resourceName = name;

        resourceLabel.text = resourceName;
    }

    // Update is called once per frame
    void Update()
    {
        if (treeHealth <= 0)
        {
            DropLog();
            Destroy(gameObject); // Destroy the tree
        }
    }

    public void TakeDamage(int damage)
    {
        treeHealth -= damage;
        Debug.Log("Tree hit! Remaining health: " + treeHealth);
    }

    void DropLog()
    {
        Instantiate(logPrefab, transform.position, Quaternion.identity);  // Drop the log at the tree's position
        Debug.Log("Log dropped!");
    }
}
