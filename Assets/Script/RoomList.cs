using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

using UnityEngine.SceneManagement;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class RoomList : MonoBehaviourPunCallbacks
{
    public static RoomList Instance;
    
    [Header("UI")] 
    public Transform roomListParent;
    public GameObject roomListItemPrefab;

    private string cachedRoomNameToCreate;

    private List<RoomInfo> cachedRoomList = new List<RoomInfo>();

    public void ChangeRoomToCreateName(string _roomName)
    {
        cachedRoomNameToCreate = _roomName;
    }

    public void CreateRoomByIndex(int _sceneIndex)
    {
        JoinRoomByName(cachedRoomNameToCreate, _sceneIndex);
    }
    
    
    private void Awake()
    {
        Instance = this;
        PhotonNetwork.AutomaticallySyncScene = true;

    }

    IEnumerator Start()
    {
        //Precaution
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
            PhotonNetwork.Disconnect();
        }

        yield return new WaitUntil(() => !PhotonNetwork.IsConnected);

        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        
        Debug.Log("Connect to server");

        PhotonNetwork.JoinLobby();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        if (cachedRoomList.Count <= 0)
        {
            cachedRoomList = roomList;
        }
        else
        {
            foreach (var room in roomList)
            {
                for (int i = 0; i < cachedRoomList.Count; i++)
                {
                    if (cachedRoomList[i].Name == room.Name)
                    {
                        List<RoomInfo> newList = cachedRoomList;

                        if (room.RemovedFromList)
                        {
                            newList.Remove(newList[i]);
                        }
                        else
                        {
                            newList[i] = room;
                        }

                        cachedRoomList = newList;

                    }
                }
            }
        }
        
        UpdateUI();
    }

    void UpdateUI()
    {
        foreach (Transform roomItem in roomListParent)
        {
            Destroy(roomItem.gameObject);
        }

        foreach (var room in cachedRoomList)
        {
            GameObject roomItem = Instantiate(roomListItemPrefab, roomListParent);

            roomItem.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = room.Name;
            roomItem.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = room.PlayerCount + "/4";

            roomItem.GetComponent<RoomItemButton>().RoomName = room.Name;
            
            int roomSceneIndex = 1;

            object sceneIndexObject;
            if(room.CustomProperties.TryGetValue("mapSceneIndex", out sceneIndexObject))
            {
                roomSceneIndex = (int)sceneIndexObject;
            }

            roomItem.GetComponent<RoomItemButton>().sceneIndex = roomSceneIndex;
        }
        
    }

    public void JoinRoomByName(string _name, int _sceneIndex)
    {
        PlayerPrefs.SetString("RoomNameToJoin", _name);
        
        gameObject.SetActive(false);
        
        SceneManager.LoadScene(_sceneIndex);
        //load relavant room
    }
    
}
