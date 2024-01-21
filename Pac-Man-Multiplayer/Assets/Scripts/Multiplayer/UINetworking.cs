using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UINetworking : MonoBehaviour
{
    public GameObject UI;
    private PhotonView photonView;

    void Start()
    {
        photonView = GetComponent<PhotonView>();
        if (!photonView.IsMine)
        {
            UI.SetActive(false);
        }
    }
}
