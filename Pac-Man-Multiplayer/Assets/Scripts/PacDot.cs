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
        SyncPacDots.IncreaseDotsRemaining();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == null)
            return;

        if (collision.tag == "PacMan")
        {
            if (PV == null)
            {
                Debug.LogError("Photon View is null");
                return;
            }

            if (collision.GetComponent<PhotonView>().IsMine)
            {
                audioManager.PlayDotSound();
            }
        
            PV.RPC("DestroyDot", RpcTarget.MasterClient); 

            // Needs syncing
            collision.GetComponent<PlayerController>().score++;
        }
    }

    [PunRPC]
    public void DestroyDot()
    {
        SyncPacDots.DecreaseDotsRemaining();
        PhotonNetwork.Destroy(gameObject);
    }
}
