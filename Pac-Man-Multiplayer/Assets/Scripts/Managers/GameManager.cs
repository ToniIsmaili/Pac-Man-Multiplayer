using Photon.Pun;
using UnityEngine;

public class GameManager : MonoBehaviourPun
{
    private PhotonView PV;
    private static NetworkManager networkManager;
    private static MapManager mapManager;

    private static bool inRoom = false;
    private static int dotsRemaining = 0;

    private void Start()
    {
        networkManager = GetComponent<NetworkManager>();
        mapManager = GameObject.Find("MapManager").GetComponent<MapManager>();
        PV = GetComponent<PhotonView>();
        networkManager.JoinRoom();
    }

    /*public void StartLevel()
    {
        if (networkManager.isMaster() && PV.IsMine)
        {
            if (mazeGenerator != null)
            {
                mazeGenerator.GenerateMaze();
                sync_map.map = mazeGenerator.getMap();
                mazeGenerator.renderMap(sync_map.map);
            }
            GetFreeTiles();
            StoreNotWalls();
            networkManager.SpawnPlayer(GetSpawnPoint());
            PlaceDot();
            dots_remaining = GameObject.FindGameObjectsWithTag("PacDot").Length;
            PlacePowerUp();
            levelLoaded = true;
        }
        else
        {
            if (sync_map.map != null)
            {
                if (sync_map.HasMapSynced())
                {
                    mazeGenerator.renderMap(sync_map.map);
                    GetFreeTiles();
                    StoreNotWalls();
                    dots_remaining = GameObject.FindGameObjectsWithTag("PacDot").Length;
                    networkManager.SpawnPlayer(GetSpawnPoint());
                    levelLoaded = true;
                } else
                {
                    Debug.Log("Failed Syncing");
                }
            }
        }
    }*/

    private void Update()
    {
        if (inRoom && dotsRemaining == 0)
        {
            mapManager.GenerateLevel(networkManager);
        }
    }

    public static void SetInRoom(bool hasJoined)
    {
        inRoom = hasJoined;
    }

    public static int GetDotsRemaining()
    {
        return dotsRemaining;
    }

    public static void SetDotsRemaining(int dotsAmount)
    {
        dotsRemaining = dotsAmount;
    }

    public static void DecreaseDotsRemaining()
    {
        dotsRemaining--;
    }
}
