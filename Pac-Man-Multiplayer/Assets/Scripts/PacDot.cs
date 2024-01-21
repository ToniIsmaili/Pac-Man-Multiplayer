using Photon.Pun;
using UnityEngine;

public class PacDot : MonoBehaviourPun
{
    private GameManager gameManager = null;
    private PhotonView PV;

    private void Start()
    {
        PV = GetComponent<PhotonView>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == null)
            return;

        if (collision.tag == "PacMan")
        {
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.Destroy(gameObject);
            }
            else
            {
                if (PV.IsMine) PV.RPC("DestroyDot", RpcTarget.MasterClient);
            }
            
            collision.GetComponent<PlayerController>().score++;
            if (gameManager != null) gameManager.dots_remaining--;
        }
    }

    [PunRPC]
    public void DestroyDot()
    {
        PhotonNetwork.Destroy(gameObject);
    }
}
