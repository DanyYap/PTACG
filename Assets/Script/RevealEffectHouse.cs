using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevealEffectHouse : MonoBehaviour
{
    public float revealSpeed = 0.5f; // Speed of the reveal effect
    private List<Material> materials = new List<Material>(); // List to store all materials

    public void GetAllMaterials()
    {
        // Get all Renderer components in this GameObject and its children
        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        // Collect instanced materials from each renderer
        foreach (Renderer renderer in renderers)
        {
            // Use renderer.material to create a unique material instance if it’s shared
            foreach (var mat in renderer.materials)
            {
                materials.Add(mat);
            }
        }
    }


    public void StartRevealCycle()
    {
        GetAllMaterials();
        StartCoroutine(RevealCycle());
    }

    private IEnumerator RevealCycle()
    {
        float revealAmount = 1f; // Start with reveal amount at 1

        // Set _RevealStrength to 1 initially for all materials
        foreach (Material mat in materials)
        {
            if (mat.HasProperty("_RevealStrength"))
            {
                mat.SetFloat("_RevealStrength", revealAmount);
            }
        }

        // Smoothly decrease revealAmount from 1 to 0
        while (revealAmount > 0f)
        {
            revealAmount -= Time.deltaTime * revealSpeed;
            revealAmount = Mathf.Clamp01(revealAmount); // Ensure it doesn't go below 0

            // Update each material’s _RevealStrength property
            foreach (Material mat in materials)
            {
                if (mat.HasProperty("_RevealStrength"))
                {
                    mat.SetFloat("_RevealStrength", revealAmount);
                }
            }

            yield return null;
        }

        // Ensure _RevealStrength is exactly 0 at the end
        foreach (Material mat in materials)
        {
            if (mat.HasProperty("_RevealStrength"))
            {
                mat.SetFloat("_RevealStrength", 0f);
            }
        }
    }
}
