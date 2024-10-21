using UnityEngine;

public class PlayerSetup : MonoBehaviour
{
    public PlayerControl control;

    public void IsLocalPlayer()
    {
        control.enabled = true;
    }
}
