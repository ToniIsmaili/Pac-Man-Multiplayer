using Photon.Pun;
using UnityEngine;

public class GameManager : MonoBehaviourPun
{
    private static NetworkManager networkManager;
    private static MapManager mapManager;

    private static bool inRoom = false;

    private void Start()
    {
        networkManager = GetComponent<NetworkManager>();
        mapManager = GameObject.Find("MapManager").GetComponent<MapManager>();
        networkManager.JoinRoom();
    }

    private void Update()
    {
        if (!inRoom)
        {
            return;
        }

        // TODO fix level generator pacdots dont line up
        if (PhotonNetwork.IsMasterClient && SyncPacDots.GetDotsRemaining() == 0)
        {
            mapManager.GenerateLevel(networkManager);
            return;
        }

        if (SyncPacDots.GetDotsRemaining() == 0 || !mapManager.PlayerInScene())
        {
            mapManager.JoinLevel(networkManager);
            return;
        }
    }

    public static void SetInRoom(bool hasJoined)
    {
        inRoom = hasJoined;
    }
}
