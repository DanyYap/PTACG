using System;
using System.Collections;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using TMPro;

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
    public GameObject InGameUI;
    public TextMeshProUGUI timerText;

    private string nickname = "unnamed";

    public string mapName = "Nothing";

    private float timer = 0f;  // Timer value
    public bool isTimerRunning = false;
    
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
            { "mapName", mapName },
            { "timer", 0f }
        };
    
        ro.CustomRoomPropertiesForLobby = new[]
        {
            "mapSceneIndex",
            "mapName",
            "timer"
        };
    
        PhotonNetwork.JoinOrCreateRoom(PlayerPrefs.GetString("RoomNameToJoin"), ro, null);
        
        nameUI.SetActive(false);
        connectingUI.SetActive(true);
    }
    
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        
        if (PhotonNetwork.IsMasterClient)
        {
            // Master client starts the timer when they join
            isTimerRunning = true;
        }
        else
        {
            // Update timer for player 2 when they join
            timer = (float)PhotonNetwork.CurrentRoom.CustomProperties["timer"];
        }
        
        SpawnPlayer();
        
        roomCam.SetActive(false);
        connectingUI.SetActive(false);
        InGameUI.SetActive(true);
        Debug.Log("We connected to a room");
        
        if (isTimerRunning)
        {
            StartCoroutine(UpdateTimer());
        }
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
    
    private IEnumerator UpdateTimer()
    {
        while (isTimerRunning)
        {
            // Exit the coroutine if the client is not connected or is leaving the room
            if (!PhotonNetwork.IsConnected || PhotonNetwork.CurrentRoom == null)
            {
                Debug.Log("Stopping UpdateTimer as the client is disconnecting or has left the room.");
                yield break;
            }

            timer += Time.deltaTime;

            try
            {
                Hashtable roomProperties = PhotonNetwork.CurrentRoom.CustomProperties;
                roomProperties["timer"] = timer;
                PhotonNetwork.CurrentRoom.SetCustomProperties(roomProperties);
            }
            catch (Exception ex)
            {
                Debug.LogWarning("Failed to update timer in room properties: " + ex.Message);
            }

            // Update the timer UI
            UpdateTimerUI();

            // Wait for the next frame
            yield return null;
        }
    }
    
    public override void OnLeftRoom()
    {
        base.OnLeftRoom();

        // Stop the timer when leaving the room
        isTimerRunning = false;
        StopAllCoroutines(); // Ensure all coroutines are stopped
        Debug.Log("Stopped all coroutines after leaving the room.");
    }

    private void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(timer / 60);
        int seconds = Mathf.FloorToInt(timer % 60);

        // Display the timer in 00:00 format
        if (timerText != null)
        {
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    [PunRPC]
    public void SyncTimer(float syncedTimer)
    {
        // Sync the timer value across all clients
        if (!PhotonNetwork.IsMasterClient)  // Ensure that only non-master clients are receiving updates
        {
            timer = syncedTimer;
        }
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
