using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public bool isInvincible = false;
    public int score = 0;

    public PowerUp powerUp = null;

    private void SpawnTile(GameObject tile)
    {
        // Spawn the tile on the player
        Instantiate(tile, transform.position, Quaternion.identity);
    }

    private void DropPowerUpCheck()
    {
        if (powerUp != null && Input.GetKeyDown(KeyCode.Q))
        {
            powerUp = null;
        }
    }

    private void UsePowerUpCheck()
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
    }

    private void Update()
    {
        // Check if the player wants to use/drop powerup
        UsePowerUpCheck();
        DropPowerUpCheck();
    }
}
