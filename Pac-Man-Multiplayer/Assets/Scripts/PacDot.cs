using Photon.Pun;
using UnityEngine;

public class PacDot : MonoBehaviourPun
{
    private AudioManager audioManager = null;
    private PhotonView PV;

    private void Start()
    {
        PV = GetComponent<PhotonView>();
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
            GameManager.DecreaseDotsRemaining();
        }
    }

    [PunRPC]
    public void DestroyDot()
    {
        PhotonNetwork.Destroy(gameObject);
    }
}
