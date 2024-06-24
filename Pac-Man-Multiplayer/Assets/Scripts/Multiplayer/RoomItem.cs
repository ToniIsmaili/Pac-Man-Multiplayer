using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class RoomItem : MonoBehaviour
{

    public Text roomName;
    private static LobbyManager lobbyManager;

    private void Start()
    {
        lobbyManager = FindObjectOfType<LobbyManager>();
    }

    public void SetName(string _roomName)
    {
        roomName.text = _roomName;
    }

    public void OnClickJoin()
    {
        lobbyManager.JoinRoom(roomName.text);
    }

}
