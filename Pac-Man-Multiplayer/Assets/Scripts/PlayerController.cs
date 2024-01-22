using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public bool isInvincible = false;
    public int score = 0;
    public Sprite sprite = null;
    public SyncPowerUp sync_powerUp = null;
    private Image inventory;
    private Tilemap tilemap;

    private void Start()
    {
        sync_powerUp = GetComponent<SyncPowerUp>();
    }

    private void SpawnTile(GameObject tile)
    {
        // Spawn the tile on the player
        Instantiate(tile, transform.position, Quaternion.identity);
    }

    private void DropPowerUpCheck()
    {
        if (sync_powerUp.powerup != null && Input.GetKeyDown(KeyCode.Q))
        {
            sync_powerUp.powerup = null;
            sprite = null;
        }
    }

    private void UsePowerUpCheck()
    {
        if (sync_powerUp.powerup != null && Input.GetKeyDown(KeyCode.Space))
        {
            if (sync_powerUp.powerup.tile_based)
            {
                SpawnTile(sync_powerUp.powerup.tile);
                sync_powerUp.powerup.StartNeutralize(gameObject, 0f);
                sync_powerUp.powerup = null;
                sprite = null;
            }
            else if (!sync_powerUp.powerup.teleport_based)
            {
                sync_powerUp.powerup.Apply(gameObject);
                sync_powerUp.powerup = null;
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
        if (sync_powerUp.powerup != null && sync_powerUp.powerup.teleport_based)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                if (GameManager.notWallTiles.Contains(tilemap.LocalToCell(pos) + new Vector3(0.5f, 0.5f, 0)))
                {
                    transform.position = tilemap.LocalToCell(pos) + new Vector3(0.5f, 0.5f, 0);
                    sync_powerUp.powerup = null;
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

        if (sync_powerUp.powerup == null)
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
