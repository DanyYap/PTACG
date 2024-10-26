using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevealEffect : MonoBehaviour
{
    public float RevealDuration = 2;
    public float RevealStrength;

    private Material revealMaterial;

    private void Start()
    {
        // Set the initial reveal strength to 0.75 for 25% visibility
        revealMaterial = GetComponent<Renderer>().material;
        revealMaterial.SetFloat("_RevealStrength", 0.75f); // 25% visible
    }

    public void StartReveal()
    {
        StartCoroutine(Revealer());
    }

    public IEnumerator Revealer()
    {
        float elapsedTime = 0;

        while (elapsedTime < RevealDuration)
        {
            elapsedTime += Time.deltaTime;

            // Lerp from 0.75 (25% visible) to 0 (fully revealed)
            RevealStrength = Mathf.Lerp(0.75f, 0, elapsedTime / RevealDuration);
            revealMaterial.SetFloat("_RevealStrength", RevealStrength);

            yield return null;
        }
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartReveal();
        }
    }
}