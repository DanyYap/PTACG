using UnityEngine;

public class RoomItemButton : MonoBehaviour
{
    public string RoomName;
    public int sceneIndex = 1;

    public void OnButtonPressed()
    {
        RoomList.Instance.JoinRoomByName(RoomName, sceneIndex);
    }
}
