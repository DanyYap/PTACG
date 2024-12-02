using System;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class PlayerSetup : MonoBehaviourPun
{
    public PlayerControl control;
    public GameObject camera;
    public AudioListener audioListener;
    
    public string nickName;

    public TextMeshPro nickNameText;

    private void Start()
    {
        if (!photonView.IsMine)
        {
            control.enabled = false; // Disable control for remote players
            camera.SetActive(false); // Disable camera for remote players
            audioListener.enabled = false; // Disable AudioListener for remote players
        }
    }

    public void IsLocalPlayer()
    {
        control.enabled = true;
        camera.SetActive(true);
    }

    [PunRPC]
    public void SetNickName(string _name)
    {
        nickName = _name;

        nickNameText.text = nickName;
    }
}
