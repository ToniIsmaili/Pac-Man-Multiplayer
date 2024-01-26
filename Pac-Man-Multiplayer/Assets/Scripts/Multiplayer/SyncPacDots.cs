using Photon.Pun;
using UnityEngine;

public class SyncPacDots : MonoBehaviour, IPunObservable
{
    private static int dotsRemaining = 0;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (PhotonNetwork.IsMasterClient && stream.IsWriting)
        {
            stream.SendNext(dotsRemaining);
        } else
        {
            dotsRemaining = (int) stream.ReceiveNext();
        }
    }

    public static int GetDotsRemaining()
    {
        return dotsRemaining;
    }

    public static void SetDotsRemaining(int dotsAmount)
    {
        dotsRemaining = dotsAmount;
    }

    public static void DecreaseDotsRemaining()
    {
        dotsRemaining--;
    }

    public static void IncreaseDotsRemaining()
    {
        dotsRemaining++;
    }
}
