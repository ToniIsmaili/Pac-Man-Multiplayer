using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPun
{
    public GameObject DeathScreen;
    private static NetworkManager networkManager;
    private static MapManager mapManager;
    private static bool inRoom = false;
    private static UIController uIController;

    private void Start()
    {
        networkManager = GetComponent<NetworkManager>();
        mapManager = GameObject.Find("MapManager").GetComponent<MapManager>();
        uIController = GameObject.FindWithTag("UserInterface").GetComponent<UIController>();
        networkManager.JoinRoom();
    }

    private void Update()
    {
        if (!inRoom)
        {
            return;
        }

        if (PhotonNetwork.IsMasterClient && SyncPacDots.GetDotsRemaining() == 0)
        {
            mapManager.GetComponent<PhotonView>().RPC("ResetMap", RpcTarget.AllBuffered);
            mapManager.GenerateLevel(networkManager);
            return;
        }

        if (!PhotonNetwork.IsMasterClient && mapManager.reset)
        {
            mapManager.JoinLevel(networkManager);
            return;
        }

        if (!mapManager.PlayerInScene())
        {
            Time.timeScale = 0;
            DeathScreen.SetActive(true);
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1;
        DeathScreen.SetActive(false);
        uIController.ResetScore();
        mapManager.GenerateLevel(networkManager);
    }

    public static void SetInRoom(bool hasJoined)
    {
        inRoom = hasJoined;
    }
}
