using Photon.Pun;

public class SyncMap : MonoBehaviourPun, IPunObservable
{
    public const int numRows = 10, numCols = 5;
    private const int mapRows = numRows * 3 + 3, mapCols = numCols * 3 + 1;
    public int[,] map = new int[mapRows, mapCols];

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    stream.SendNext(map[i, j]);
                }
            }
        }
        else if (stream.IsReading)
        {
            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    map[i, j] = (int)stream.ReceiveNext();
                }
            }
        }
    }

    public bool HasMapSynced()
    {
        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                if (map[i, j] == -1) return true;
            }
        }

        return false;
    }
}
