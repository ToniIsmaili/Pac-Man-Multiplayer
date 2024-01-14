using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    private static MazeGenerator mazeGenerator;
    private List<Vector3> freeTiles = new List<Vector3>();
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
        if (mazeGenerator != null) mazeGenerator.GenerateMaze();
        GetFreeTiles();
        SpawnPlayer(false);
        PlaceDot();
        dots_remaining = GameObject.FindGameObjectsWithTag("PacDot").Length;
        PlacePowerUp();
    }

    private void Update()
    {
        if (ZeroDots(dots_remaining)) GenerateLevel();
    }

    private void GenerateLevel()
    {
        DestroyAllTags("PowerUp");
        tilemap.ClearAllTiles();
        if (mazeGenerator != null) mazeGenerator.GenerateMaze();
        GetFreeTiles();
        SpawnPlayer(true);
        PlaceDot();
        dots_remaining = GameObject.FindGameObjectsWithTag("PacDot").Length;
        PlacePowerUp();
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
                Instantiate(dots, v, Quaternion.identity);
                freeTiles.Remove(v);
            } else if (Random.Range(0, 100) > power_up_chance) {
                // If Random.Range is greater than power_up_chance spawn dot
                Instantiate(dots, v, Quaternion.identity);
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
            Instantiate(powerup[Random.Range(0, powerups)], v, Quaternion.identity);
            freeTiles.Remove(v);
        }
    }

    // Spawns player at random free tile
    private void SpawnPlayer(bool isPlayerInScene)
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

    private bool isGhostSpawn(int i, int j)
    {
        if (i == 1 && j == 2)
        {
            Instantiate(barrier, new Vector3(i + 0.5f, j + 0.5f, 0), Quaternion.identity);
            return false;
        }
        if (i == 0 && j == 2)
        {
            Instantiate(barrier, new Vector3(i + 0.5f, j + 0.5f, 0), Quaternion.identity);
            return false;
        }
        if (i == -1 && j == 2)
        {
            Instantiate(barrier, new Vector3(i + 0.5f, j + 0.5f, 0), Quaternion.identity);
            return false;
        }
        if (i <= 3 && i >= -3 && j <= 1 && j >= -1) return false;
        return true;
    }

}
