using Unity.VisualScripting;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public PowerUp powerUp = null;  

    private void SpawnTile(GameObject tile)
    {
        // Spawn the tile on the player
        Instantiate(tile, transform.position, Quaternion.identity);
    }
    private void Update()
    {
        if (powerUp != null && Input.GetKeyDown(KeyCode.Space))
        {
            if (powerUp.tile_based)
            {
                SpawnTile(powerUp.tile);
                powerUp.StartNeutralize(gameObject, 0f);
                powerUp = null; 
            }
            else
            {
                powerUp.Apply(gameObject);
                powerUp = null;
            }
        }

        if (powerUp != null && Input.GetKeyDown(KeyCode.Q))
        {
            powerUp = null;
        }
    }

}
