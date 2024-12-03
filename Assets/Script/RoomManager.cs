using System;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager instance;
    
    public GameObject player;

    [Space] 
    public Transform[] spawnPoints;

    [Space] 
    public GameObject roomCam;

    [Space] 
    public GameObject nameUI;
    public GameObject connectingUI;

    private string nickname = "unnamed";

    public string mapName = "Nothing";

    private void Awake()
    {
        instance = this;
    }

    public void ChangeNickName(string _name)
    {
        nickname = _name;
    }

    public void JoinRoomButtonPressed()
    {
        Debug.Log("Connecting...");
    
        RoomOptions ro = new RoomOptions();
    
        ro.CustomRoomProperties = new Hashtable()
        {
            { "mapSceneIndex", SceneManager.GetActiveScene().buildIndex },
            { "mapName", mapName }
        };
    
        ro.CustomRoomPropertiesForLobby = new[]
        {
            "mapSceneIndex",
            "mapName"
        };
    
        PhotonNetwork.JoinOrCreateRoom(PlayerPrefs.GetString("RoomNameToJoin"), ro, null);
        
        nameUI.SetActive(false);
        connectingUI.SetActive(true);
    }
    
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        SpawnPlayer();
        
        roomCam.SetActive(false);
        connectingUI.SetActive(false);
        Debug.Log("We connected to a room");
    }

    public void SpawnPlayer()
    {
        Transform spawnPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];
    
        // Instantiate the correct player prefab
        GameObject _player = PhotonNetwork.Instantiate(player.name, spawnPoint.position, Quaternion.identity);
        _player.GetComponent<PlayerSetup>().IsLocalPlayer();
    
         _player.GetComponent<PhotonView>().RPC("SetNickName", RpcTarget.AllBuffered, nickname);
         PhotonNetwork.LocalPlayer.NickName = nickname;

    }

    public void SetHashes()
    {
        try
        {
            Hashtable hash = PhotonNetwork.LocalPlayer.CustomProperties;

            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
        catch
        {
            //do nothing
        }
    }

    public void RespawnPlayer()
    {
        Transform spawnPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];
        
        GameObject _player = PhotonNetwork.Instantiate(player.name, spawnPoint.position, Quaternion.identity);
        _player.GetComponent<PlayerSetup>().IsLocalPlayer();
        
        _player.GetComponent<PhotonView>().RPC("SetNickName", RpcTarget.AllBuffered, nickname);
        PhotonNetwork.LocalPlayer.NickName = nickname;
    }
}
