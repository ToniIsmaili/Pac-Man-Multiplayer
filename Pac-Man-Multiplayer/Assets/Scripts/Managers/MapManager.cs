using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    private static MazeGenerator mazeGenerator;
    private static NetworkManager networkManager;
    private SyncMap sync_map;

    private static List<Vector3> freeTiles = new List<Vector3>();
    public static List<Vector3> notWallTiles = new List<Vector3>();

    [Header("Power Ups")]
    [Tooltip("The % chance for a Power Up to spawn.")]
    public int powerUpChance = 5;
    [Tooltip("List of possible Power Ups that should be able to spawn in the level. (Insert prefabs of power ups)")]
    public GameObject[] powerUps;
    // Used to force dots, and spreading power ups out.
    private bool force_dot = false;

    void Start()
    {
        mazeGenerator = GetComponent<MazeGenerator>();
        networkManager = GetComponent<NetworkManager>();
        sync_map = GetComponent<SyncMap>();
    }

    void Update()
    {
        
    }

    // Generates a random level (should only be run on the master client)
    private void GenerateLevel()
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

        // Generates a 2D array of the map ( -1 = empty & 0 = tile )
        mazeGenerator.GenerateMaze();

        // Synchronizes the map with all the players and instantiates it locally
        sync_map.map = mazeGenerator.getMap();
        mazeGenerator.renderMap(sync_map.map);

        GetFreeTiles();
        StoreNotWalls();
        // networkManager.SpawnPlayer(GetSpawnPoint());
        PlaceDot();
        PlacePowerUp();
        // dots_remaining = GameObject.FindGameObjectsWithTag("PacDot").Length;

    }

    // Stores all the positions of not wall tiles in the list.
    private void GetFreeTiles()
    {
        for (int i = mazeGenerator.tilemap.cellBounds.position.x; i < mazeGenerator.tilemap.cellBounds.size.x + mazeGenerator.tilemap.cellBounds.position.x; i++)
        {
            for (int j = mazeGenerator.tilemap.cellBounds.position.y; j < mazeGenerator.tilemap.cellBounds.size.y + mazeGenerator.tilemap.cellBounds.position.y; j++)
            {
                if (mazeGenerator.tilemap.GetTile(new Vector3Int(i, j, 0)) == null)
                {
                    if (isGhostSpawn(i, j) && isTunnel(i, j)) freeTiles.Add(new Vector3(i, j, 0));
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

    private bool isTunnel(int i, int j)
    {
        if (i == mazeGenerator.tilemap.cellBounds.position.x)
        {
            networkManager.Spawn("Tunnel", new Vector3(i + 0.5f, j + 0.5f, 0), Quaternion.Euler(0, 0, 0));
            return false;
        }

        if (i == mazeGenerator.tilemap.cellBounds.size.x + mazeGenerator.tilemap.cellBounds.position.x - 1)
        {
            networkManager.Spawn("Tunnel", new Vector3(i + 0.5f, j + 0.5f, 0), Quaternion.Euler(0, 0, -180));
            return false;
        }

        return true;
    }

    private bool isGhostSpawn(int i, int j)
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

    private void PlaceDot()
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
                // Doesnt spawn dot (free for powerup)
                force_dot = true;
            }
        }
    }

    private void PlacePowerUp()
    {
        int powerups = powerUps.Length;

        foreach (Vector3 v in freeTiles.ToList())
        {
            networkManager.SpawnPowerUp(powerUps[Random.Range(0, powerups)].name, v);
            freeTiles.Remove(v);
        }
    }

}
