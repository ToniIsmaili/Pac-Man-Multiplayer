using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    public int dots_remaining;
    public Tilemap tilemap;
    public GameObject dots;
    void Start()
    {
        PlaceDot();
        dots_remaining = GameObject.FindGameObjectsWithTag("PacDot").Length;
    }

    private void Update()
    {
        //PlaceDot();
    }

    private void PlaceDot()
    {
        //Debug.Log(tilemap.cellBounds);
        for (int i = tilemap.cellBounds.position.x; i < tilemap.cellBounds.size.x + tilemap.cellBounds.position.x; i++)
        {
            for(int j = tilemap.cellBounds.position.y; j < tilemap.cellBounds.size.y + tilemap.cellBounds.position.y; j++)
            {

                //Debug.Log(tilemap.GetTile(new Vector3Int(i, j, 0)));
                if (tilemap.GetTile(new Vector3Int(i, j, 0)) == null)
                    Instantiate(dots, new Vector3(i + 0.5f, j + 0.5f, 0), Quaternion.identity);

            }
        }
    }

}
