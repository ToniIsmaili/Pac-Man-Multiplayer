using Photon.Pun;
using UnityEngine;

public class PacDot : MonoBehaviourPun
{
    private GameManager gameManager = null;
    private AudioManager audioManager = null;
    private PhotonView PV;

    private void Start()
    {
        PV = GetComponent<PhotonView>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == null)
            return;

        if (collision.tag == "PacMan")
        {
            if (collision.GetComponent<PhotonView>().IsMine)
            {
                audioManager.PlayDotSound();
            }

            if (PV.IsMine)
            {
                PhotonNetwork.Destroy(gameObject);
            }
            else
            {
                PV.RPC("DestroyDot", RpcTarget.MasterClient);
            }

            // Needs syncing
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
