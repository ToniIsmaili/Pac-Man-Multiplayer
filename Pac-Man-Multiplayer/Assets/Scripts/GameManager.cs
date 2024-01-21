using Photon.Pun;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviourPun
{
    private PhotonView PV;
    private static MazeGenerator mazeGenerator;
    private static NetworkManager networkManager;
    private SyncMap sync_map;
    private static List<Vector3> freeTiles = new List<Vector3>();
    public static List<Vector3> notWallTiles = new List<Vector3>();
    private static bool inRoom = false, levelLoaded = false;
    private bool force_dot = false;
    public int power_up_chance = 5;
    public int dots_remaining;
    public Tilemap tilemap;
    public Tilemap corridor;
    public GameObject[] powerup;
    public GameObject dots;
    public GameObject player;
    public GameObject barrier;
    public GameObject tunnel;

    private void Start()
    {
        mazeGenerator = GetComponent<MazeGenerator>();
        networkManager = GetComponent<NetworkManager>();
        sync_map = GetComponent<SyncMap>();
        PV = GetComponent<PhotonView>();
        networkManager.JoinRoom();
    }

    public void StartLevel()
    {
        if (networkManager.isMaster() && PV.IsMine)
        {
            if (mazeGenerator != null)
            {
                mazeGenerator.GenerateMaze();
                sync_map.map = mazeGenerator.getMap();
                mazeGenerator.renderMap(sync_map.map);
            }
            GetFreeTiles();
            StoreNotWalls();
            networkManager.SpawnPlayer(GetSpawnPoint());
            PlaceDot();
            dots_remaining = GameObject.FindGameObjectsWithTag("PacDot").Length;
            PlacePowerUp();
            levelLoaded = true;
        }
        else
        {
            if (sync_map.map != null)
            {
                if (sync_map.HasMapSynced())
                {
                    mazeGenerator.renderMap(sync_map.map);
                    GetFreeTiles();
                    StoreNotWalls();
                    dots_remaining = GameObject.FindGameObjectsWithTag("PacDot").Length;
                    networkManager.SpawnPlayer(GetSpawnPoint());
                    levelLoaded = true;
                } else
                {
                    Debug.Log("Failed Syncing");
                }
            }
        }
    }

    private void Update()
    {
        if (inRoom && !levelLoaded)
        {
            StartLevel();
        }

        if (ZeroDots(dots_remaining) && levelLoaded)
        {
            GenerateLevel();
        }
    }

    public static void hasJoinedRoom(bool hasJoined)
    {
        inRoom = hasJoined;
    }

    private void GenerateLevel()
    {
        if (networkManager.isMaster())
        {
            DestroyAllTags("PowerUp");
            tilemap.ClearAllTiles();
            if (mazeGenerator != null) mazeGenerator.GenerateMaze();
            GetFreeTiles();
            StoreNotWalls();
            // SpawnPlayer(true);
            networkManager.SpawnPlayer(GetSpawnPoint());
            networkManager.SpawnPlayer(GetSpawnPoint());
            PlaceDot();
            dots_remaining = GameObject.FindGameObjectsWithTag("PacDot").Length;
            PlacePowerUp();
        } else
        {
            dots_remaining = GameObject.FindGameObjectsWithTag("PacDot").Length;
            GetFreeTiles();
            StoreNotWalls();
            networkManager.SpawnPlayer(GetSpawnPoint());
        }
    }

    private bool ZeroDots(int dots_remaining)
    {
        return dots_remaining == 0;
    }

    private void GetFreeTiles()
    {
        for (int i = tilemap.cellBounds.position.x; i < tilemap.cellBounds.size.x + tilemap.cellBounds.position.x; i++)
        {
            for (int j = tilemap.cellBounds.position.y; j < tilemap.cellBounds.size.y + tilemap.cellBounds.position.y; j++)
            {
                if (tilemap.GetTile(new Vector3Int(i, j, 0)) == null)
                {
                    if (isGhostSpawn(i, j) && isTunnel(i, j)) freeTiles.Add(new Vector3(i, j, 0));
                }
            }
        }

        for (int i = 0; i < freeTiles.Count; i++)
        {
            Vector3 v = freeTiles[i];
            v.x += 0.5f;
            v.y += 0.5f;
            freeTiles[i] = v;
        }
    }

    private void PlaceDot()
    {
        foreach (Vector3 v in freeTiles.ToList())
        {
            if (force_dot) {
                // Spawns a dot by force
                force_dot = false;
                //Instantiate(dots, v, Quaternion.identity);
                networkManager.SpawnDot(v);
                freeTiles.Remove(v);
            } else if (Random.Range(0, 100) > power_up_chance) {
                // If Random.Range is greater than power_up_chance spawn dot
                //Instantiate(dots, v, Quaternion.identity);
                networkManager.SpawnDot(v);
                freeTiles.Remove(v);
            } else {
                // Doesnt spawn dot (free for powerup)
                force_dot = true;
            }
        }
    }

    private void PlacePowerUp()
    {
        int powerups = powerup.Length;

        foreach (Vector3 v in freeTiles.ToList())
        {
            networkManager.SpawnPowerUp(powerup[Random.Range(0, powerups)].name, v);
            freeTiles.Remove(v);
        }
    }

    // Spawns player at random free tile
    /*private void SpawnPlayer(bool isPlayerInScene)
    {
        int index = Random.Range(0, freeTiles.Count - 1);
        if (player != null)
        {
            if (isPlayerInScene)
            {
                // Might cause problem with multiplayer
                GameObject.FindGameObjectWithTag("PacMan").transform.position = freeTiles[index];
            } else
            {
                Instantiate(player, freeTiles[index], Quaternion.identity);
            }

            freeTiles.RemoveAt(index);
        }
    }*/

    public static Vector3 GetSpawnPoint()
    {
        Vector2 spawnPoint = freeTiles[Random.Range(0, freeTiles.Count - 1)];
        freeTiles.Remove(spawnPoint);
        return spawnPoint;
    }

    private static void DestroyAllTags(string tag)
    {
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag(tag);
        foreach (GameObject go in gameObjects)
        {
            Destroy(go);
        }
    }

    private bool isTunnel(int i, int j)
    {

        if (i == tilemap.cellBounds.position.x)
        {
            Instantiate(tunnel, new Vector3(i + 0.5f, j + 0.5f, 0), Quaternion.Euler(0, 0, 0));
            return false;
        }

        if (i == tilemap.cellBounds.size.x + tilemap.cellBounds.position.x - 1)
        {
            Instantiate(tunnel, new Vector3(i + 0.5f, j + 0.5f, 0), Quaternion.Euler(0, 0, -180));
            return false;
        }

        return true;
    }

    private void StoreNotWalls()
    {
        foreach(Vector3 v in freeTiles)
        {
            notWallTiles.Add(v);
        }
    }

    private bool isGhostSpawn(int i, int j)
    {
        if (i == 1 && j == 2)
        {
            networkManager.SpawnBarrier(new Vector3(i + 0.5f, j + 0.5f, 0));
            // Instantiate(barrier, new Vector3(i + 0.5f, j + 0.5f, 0), Quaternion.identity);
            return false;
        }
        if (i == 0 && j == 2)
        {
            networkManager.SpawnBarrier(new Vector3(i + 0.5f, j + 0.5f, 0));
            // Instantiate(barrier, new Vector3(i + 0.5f, j + 0.5f, 0), Quaternion.identity);
            return false;
        }
        if (i == -1 && j == 2)
        {
            networkManager.SpawnBarrier(new Vector3(i + 0.5f, j + 0.5f, 0));
            // Instantiate(barrier, new Vector3(i + 0.5f, j + 0.5f, 0), Quaternion.identity);
            return false;
        }
        if (i <= 3 && i >= -3 && j <= 1 && j >= -1) return false;
        return true;
    }

}
