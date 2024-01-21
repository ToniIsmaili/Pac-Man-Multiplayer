using Photon.Realtime;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public bool isInvincible = false;
    public int score = 0;
    public Sprite sprite = null;
    public PowerUp powerUp = null;
    private Image inventory;
    private Tilemap tilemap;

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
            sprite = null;
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
                sprite = null;
            }
            else if (!powerUp.teleport_based)
            {
                powerUp.Apply(gameObject);
                powerUp = null;
                sprite = null;
            }
        }
    }

    private void UseTeleport()
    {
        if (tilemap == null)
        {
            tilemap = GameObject.Find("GameManager").GetComponent<GameManager>().tilemap;
        }
        if (powerUp != null && powerUp.teleport_based)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                if (GameManager.notWallTiles.Contains(tilemap.LocalToCell(pos) + new Vector3(0.5f, 0.5f, 0)))
                {
                    transform.position = tilemap.LocalToCell(pos) + new Vector3(0.5f, 0.5f, 0);
                    powerUp = null;
                    sprite = null;
                }
            }
        }
    }

    private void UpdateInventory()
    {
        if (inventory == null)
        {
            inventory = GameObject.Find("Inventory").GetComponent<Image>();
        }

        if (powerUp == null)
            inventory.sprite = null;
        else inventory.sprite = sprite;
    }

    private void Update()
    {
        // Check if the player wants to use/drop powerup
        UsePowerUpCheck();
        DropPowerUpCheck();
        UseTeleport();
        UpdateInventory();
    }
}
