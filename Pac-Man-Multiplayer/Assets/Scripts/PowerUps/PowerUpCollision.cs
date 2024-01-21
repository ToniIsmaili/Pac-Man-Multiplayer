using Photon.Pun;
using UnityEngine;

public class PowerUpCollision : MonoBehaviourPun
{
    public PowerUp powerUp;
    private PhotonView PV;

    private void Start()
    {
        PV = GetComponent<PhotonView>();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == "PacMan" && collider.GetComponent<PlayerController>().powerUp == null)
        {
            Debug.Log("Collided with player");
            collider.GetComponent<PlayerController>().powerUp = powerUp;
            collider.GetComponent <PlayerController>().sprite = gameObject.GetComponent<SpriteRenderer>().sprite;
            
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.Destroy(gameObject);
            } else
            {
                if (PV.IsMine) PV.RPC("DestroyPowerUp", RpcTarget.MasterClient);
            }
        }
    }

    [PunRPC]
    public void DestroyPowerUp()
    {
        Debug.Log("Destroyed");
        PhotonNetwork.Destroy(gameObject);
    }

}
