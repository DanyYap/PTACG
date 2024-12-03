using System;
using UnityEngine;

public class PressureButtonSystem : MonoBehaviour
{
    public Animator platformAnimation;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            platformAnimation.SetBool("isPressing", true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            platformAnimation.SetBool("isPressing", false);
        }
    }
}
