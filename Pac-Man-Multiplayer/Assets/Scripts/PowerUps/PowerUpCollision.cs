using Photon.Pun;
using UnityEngine;

public class PowerUpCollision : MonoBehaviourPun
{
    public PowerUp powerUp;
    private PhotonView PV;
    private AudioManager audioManager = null;

    private void Start()
    {
        PV = GetComponent<PhotonView>();
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == "PacMan" && collider.GetComponent<SyncPowerUp>().powerup == null)
        {
            int playerId = collider.GetComponent<PhotonView>().ViewID;

            if (collider.GetComponent<PhotonView>().IsMine)
            {
                audioManager.PlayPowerUpSound();
            }

            PV.RPC("SetPowerUp", RpcTarget.All, playerId);
            collider.GetComponent <PlayerController>().sprite = gameObject.GetComponent<SpriteRenderer>().sprite;

            if (PV.IsMine)
            {
                PhotonNetwork.Destroy(gameObject);
            } else
            {
                PV.RPC("DestroyPowerUp", RpcTarget.MasterClient);
            }
        }
    }

    [PunRPC]
    public void DestroyPowerUp()
    {
        PhotonNetwork.Destroy(gameObject);
    }

    [PunRPC]
    public void SetPowerUp(int playerId)
    {
        GameObject player = PhotonView.Find(playerId).gameObject;
        player.GetComponent<SyncPowerUp>().Sync(powerUp.name);
    }

}
