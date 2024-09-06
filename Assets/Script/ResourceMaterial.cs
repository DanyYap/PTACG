using UnityEngine;
using TMPro;

public class ResourceMaterial : MonoBehaviour
{
    public TMP_Text resourceLabel;
    
    
    // Start is called before the first frame update
    void Start()
    {
        string resourceName = name;

        resourceLabel.text = resourceName;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
