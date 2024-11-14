using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;

public class CreateRoomMenu : MonoBehaviourPunCallbacks
{
    [SerializeField] private TextMeshProUGUI _roomName;

    public void OnClick_CreateRoom()
    {
        if (PhotonNetwork.IsConnected)
            return;
        
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 2;
        PhotonNetwork.JoinOrCreateRoom(_roomName.text, options, TypedLobby.Default);
    }

    public override void OnCreatedRoom()
    {
        MasterManager.DebugConsole.AddText("Create Romm Successfully.", this);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        MasterManager.DebugConsole.AddText("Room Create Failed: " + message, this);
    }
    
}
