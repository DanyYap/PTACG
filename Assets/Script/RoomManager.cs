using System;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager instance;
    
    public GameObject player1;
    public GameObject player2;

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
    
        // Check if the local player is the master client
        if (PhotonNetwork.IsMasterClient)
        {
            SpawnPlayer(player1);
            Debug.Log("Master client has joined as Player 1");
        }
        else
        {
            SpawnPlayer(player2);
            Debug.Log("Second client has joined as Player 2");
        }

        roomCam.SetActive(false);
        connectingUI.SetActive(false);
        Debug.Log("We connected to a room");
    }

    public void SpawnPlayer(GameObject playerPrefab)
    {
        Transform spawnPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];
    
        // Instantiate the correct player prefab
        GameObject _player = PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint.position, Quaternion.identity);
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
}
