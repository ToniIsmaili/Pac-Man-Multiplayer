using Photon.Pun;
using UnityEngine;

public class PlayerNetworking : MonoBehaviour
{
    public MonoBehaviour[] ignoreScripts;
    private PhotonView photonView;

    void Start()
    {
        photonView = GetComponent<PhotonView>();
        if (!photonView.IsMine)
        {
            foreach (MonoBehaviour script in ignoreScripts)
            {
                script.enabled = false;
            }
        }
    }
}
