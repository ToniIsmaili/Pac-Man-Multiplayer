using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    private Vector3 spawnpoint;
    public void JoinRoom()
    {
        // this.spawnpoint = spawnpoint;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions { MaxPlayers = 4 }, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined room");
        GameManager.hasJoinedRoom(true);
    }

    public void SpawnPlayer(Vector3 spawnpoint)
    {
        PhotonNetwork.Instantiate("Player", spawnpoint, Quaternion.identity);
    }

    public void SpawnDot(Vector3 vector)
    {
        PhotonNetwork.Instantiate("PacDot", vector, Quaternion.identity);
    }

    public void SpawnBarrier(Vector3 vector)
    {
        PhotonNetwork.Instantiate("Barrier", vector, Quaternion.identity);
    }

    public void SpawnPowerUp(string powerUp, Vector3 vector)
    {
        PhotonNetwork.Instantiate(powerUp, vector, Quaternion.identity);
    }

    IEnumerator Coroutine()
    {
        yield return new WaitForSeconds(5);
    }

    public bool isMaster()
    {
        return PhotonNetwork.IsMasterClient;
    }
}
