using Photon.Pun;
using Photon.Realtime;
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
        GameManager.SetInRoom(true);
    }

    public GameObject Spawn(string prefabName, Vector3 position)
    {
        return PhotonNetwork.Instantiate(prefabName, position, Quaternion.identity);
    }

    public GameObject Spawn(string prefabName, Vector3 position, Quaternion rotation)
    {
        return PhotonNetwork.Instantiate(prefabName, position, rotation);
    }

    public void DestroyAllTags(string tag)
    {
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag(tag);
        foreach (GameObject go in gameObjects)
        {
            PhotonNetwork.Destroy(go);
        }
    }

    public bool isMaster()
    {
        return PhotonNetwork.IsMasterClient;
    }
}
