using System;
using UnityEngine;
using Photon.Pun;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager instance;
    
    public GameObject player;

    [Space] 
    public Transform[] spawnPoints;

    [Space] 
    public GameObject roomCam;
    public GameObject levelCam;

    [Space] 
    public GameObject nameUI;
    public GameObject connectingUI;

    private string nickname = "unnamed";

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

        PhotonNetwork.ConnectUsingSettings();
        
        nameUI.SetActive(false);
        connectingUI.SetActive(true);
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        
        Debug.Log("Connected to Server");

        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        
        Debug.Log("We Joined the Lobby");

        PhotonNetwork.JoinOrCreateRoom("test", null, null);
    }
    
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        
        Debug.Log("We connected to a room");
        
        roomCam.SetActive(false);
        levelCam.SetActive(true);
        
        SpawnPlayer();
    }

    public void SpawnPlayer()
    {
        Transform spawnPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];
        
        GameObject _player = PhotonNetwork.Instantiate(player.name, spawnPoint.position, Quaternion.identity);
        _player.GetComponent<PlayerSetup>().IsLocalPlayer();
        
        _player.GetComponent<PhotonView>().RPC("SetNickName", RpcTarget.AllBuffered, nickname);
    }
}
