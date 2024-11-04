using Photon.Pun;
using TMPro;
using UnityEngine;

public class PlayerSetup : MonoBehaviour
{
    public PlayerControl control;
    public GameObject camera;

    public string nickName;

    public TextMeshPro nickNameText;

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
