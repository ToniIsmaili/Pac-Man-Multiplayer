using Photon.Pun;
using UnityEngine;

public class PacDot : MonoBehaviourPun
{
    private UIController uIController;
    private AudioManager audioManager = null;
    private PhotonView PV;

    private void Start()
    {
        PV = GetComponent<PhotonView>();
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        uIController = GameObject.FindWithTag("UserInterface").GetComponent<UIController>();
        SyncPacDots.IncreaseDotsRemaining();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == null)
            return;

        if (collision.CompareTag("PacMan"))
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

            if (uIController != null) uIController.UpdateScore();
        }
    }

    [PunRPC]
    public void DestroyDot()
    {
        SyncPacDots.DecreaseDotsRemaining();
        PhotonNetwork.Destroy(gameObject);
    }
}
