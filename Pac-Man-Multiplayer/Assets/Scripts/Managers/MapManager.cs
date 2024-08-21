using Photon.Pun;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapManager : MonoBehaviourPun
{
    private static MazeGenerator mazeGenerator;
    private SyncMap sync_map;

    private static List<Vector3> freeTiles = new List<Vector3>();

    private GameObject player = null;
    // Used to force dots, and spreading power ups out.
    private bool force_dot = false;
    public static List<Vector3> notWallTiles = new List<Vector3>();
    public bool reset = true;

    [Header("Power Ups")]
    [Tooltip("The % chance for a Power Up to spawn.")]
    public int powerUpChance = 5;
    [Tooltip("List of possible Power Ups that should be able to spawn in the level. (Insert prefabs of power ups)")]
    public GameObject[] powerUps;

    void Start()
    {
        mazeGenerator = GetComponent<MazeGenerator>();
        sync_map = GetComponent<SyncMap>();
    }

    // Generates a random level (should only be run on the master client)
    public void GenerateLevel(NetworkManager networkManager)
    {
        if (mazeGenerator == null)
        {
            Debug.LogError("Maze Generator is null! - MapManager");
        }
        if (networkManager == null)
        {
            Debug.LogError("Network Manager is null! - MapManager");
        }

        // Resets the level by destroying all Power Ups and Tiles, should only run when there
        // are no more PacDots remaining or at the start of the game.
        networkManager.DestroyAllTags("PowerUp");
        mazeGenerator.ClearTileMap();

        // Generates a 2D array of the map ( -1 = tile & 0 = empty )
        mazeGenerator.GenerateMaze();

        // Synchronizes the map with all the players and instantiates it locally
        sync_map.map = mazeGenerator.getMap();
        mazeGenerator.renderMap(sync_map.map);

        // Fills freeTiles and notWallTiles lists with positions
        GetFreeTiles(networkManager);
        StoreNotWalls();

        HandlePlayer(networkManager);

        networkManager.DestroyAllTags("PacDot");
        PlaceDot(networkManager);
        PlacePowerUp(networkManager);
    }

    // Joins the level (should be run on other clients)
    public void JoinLevel(NetworkManager networkManager)
    {
        if (sync_map.map == null)
        {
            Debug.LogError("sync_map.map is null! - MapManager");
            return;
        }

        // If the map hasn't synchronized, stop the function
        if (!sync_map.HasMapSynced())
        {
            return;
        }

        // Resets the level by destroying all Tiles, should only run when there
        // are no more PacDots remaining or at the start of the game.
        mazeGenerator.ClearTileMap();

        // Instantiates the map locally
        mazeGenerator.renderMap(sync_map.map);

        // Fills freeTiles and notWallTiles lists with positions
        GetFreeTiles(networkManager);
        StoreNotWalls();

        HandlePlayer(networkManager);
        reset = false;
    }

    public bool PlayerInScene()
    {
        return player != null;
    }

    [PunRPC]
    public void ResetMap()
    {
        sync_map.ResetMap();
        reset = true;
    }

    // Handles the spawning/teleporting of the player at the beginning of a level
    private void HandlePlayer(NetworkManager networkManager)
    {
        if (player == null)
        {
            player = networkManager.Spawn("Player", GetSpawnPoint());
            return;
        }

        player.transform.position = GetSpawnPoint();
    }

    // Returns a random position from freeTiles list and then removes it from the list.
    private static Vector3 GetSpawnPoint()
    {
        Vector2 spawnPoint = freeTiles[Random.Range(0, freeTiles.Count - 1)];
        freeTiles.Remove(spawnPoint);
        return spawnPoint;
    }

    // Stores all the positions of not wall tiles in the list.
    private void GetFreeTiles(NetworkManager networkManager)
    {
        for (int i = mazeGenerator.tilemap.cellBounds.position.x; i < mazeGenerator.tilemap.cellBounds.size.x + mazeGenerator.tilemap.cellBounds.position.x; i++)
        {
            for (int j = mazeGenerator.tilemap.cellBounds.position.y; j < mazeGenerator.tilemap.cellBounds.size.y + mazeGenerator.tilemap.cellBounds.position.y; j++)
            {
                if (mazeGenerator.tilemap.GetTile(new Vector3Int(i, j, 0)) == null)
                {
                    if (isGhostSpawn(networkManager, i, j) && isTunnel(networkManager, i, j)) freeTiles.Add(new Vector3(i, j, 0));
                }
            }
        }

        // Increment each position by 0.5 so objects spawned at those locations are in the center
        for (int i = 0; i < freeTiles.Count; i++)
        {
            Vector3 v = freeTiles[i];
            v.x += 0.5f;
            v.y += 0.5f;
            freeTiles[i] = v;
        }
    }

    // Stores all the positions from freeTiles list for later use (ex Teleport Power Up)
    private void StoreNotWalls()
    {
        foreach (Vector3 position in freeTiles)
        {
            notWallTiles.Add(position);
        }
    }

    // Checks if coordinates are supposed to be tunnels, if yes it returns false meaning those coordinates
    // shouldn't be saved in the freeTiles list
    private bool isTunnel(NetworkManager networkManager, int i, int j)
    {
        if (i == mazeGenerator.tilemap.cellBounds.position.x + 1)
        {
            networkManager.Spawn("Tunnel", new Vector3(i + 0.5f, j + 0.5f, 0), Quaternion.Euler(0, 0, 0));
            return false;
        }

        if (i == mazeGenerator.tilemap.cellBounds.size.x + mazeGenerator.tilemap.cellBounds.position.x - 2)
        {
            networkManager.Spawn("Tunnel", new Vector3(i + 0.5f, j + 0.5f, 0), Quaternion.Euler(0, 0, -180));
            return false;
        }

        return true;
    }

    // Checks if coordinates are supposed to be ghosts spawn, if yes it returns false meaning those coordinates
    // shouldn't be saved in the freeTiles list
    private bool isGhostSpawn(NetworkManager networkManager, int i, int j)
    {
        if (i == 1 && j == 2)
        {
            networkManager.Spawn("Barrier", new Vector3(i + 0.5f, j + 0.5f, 0));
            return false;
        }
        if (i == 0 && j == 2)
        {
            networkManager.Spawn("Barrier", new Vector3(i + 0.5f, j + 0.5f, 0));
            return false;
        }
        if (i == -1 && j == 2)
        {
            networkManager.Spawn("Barrier", new Vector3(i + 0.5f, j + 0.5f, 0));
            return false;
        }
        if (i <= 3 && i >= -3 && j <= 1 && j >= -1) return false;
        return true;
    }

    // Places PacDot in the positions stored in freeTiles (then removing that position), with a chance to not place a dot in a
    // certain position, leaving it blank for a power up
    private void PlaceDot(NetworkManager networkManager)
    {
        foreach (Vector3 position in freeTiles.ToList())
        {
            if (force_dot)
            {
                // Spawns a dot by force
                force_dot = false;
                networkManager.Spawn("PacDot", position);
                freeTiles.Remove(position);
            }
            else if (Random.Range(0, 100) > powerUpChance)
            {
                // If Random.Range is greater than power_up_chance spawn dot
                networkManager.Spawn("PacDot", position);
                freeTiles.Remove(position);
            }
            else
            {
                // Doesnt spawn dot (free for powerup), forces dot to spread out power ups
                force_dot = true;
            }
        }
    }

    // Places a random power up from the list of power ups in all the positions left in freeTiles
    private void PlacePowerUp(NetworkManager networkManager)
    {
        int powerups = powerUps.Length;

        foreach (Vector3 position in freeTiles.ToList())
        {
            networkManager.Spawn(powerUps[Random.Range(0, powerups)].name, position);
            freeTiles.Remove(position);
        }
    }

}
