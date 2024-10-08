using UnityEngine;

public class PlayerSetup : MonoBehaviour
{
    public PlayerControl control;
    public GameObject playerCamera;

    public void IsLocalPlayer()
    {
        control.enabled = true;
        playerCamera.SetActive(true);
    }
}
