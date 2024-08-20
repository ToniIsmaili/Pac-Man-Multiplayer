using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostManager : MonoBehaviour
{
    public GameObject ghostRedPrefab;
    public GameObject ghostPinkPrefab;
    public GameObject ghostCyanPrefab;
    public GameObject ghostOrangePrefab;
    public Vector3 redSpawnPoint;
    public Vector3 pinkSpawnPoint;
    public Vector3 cyanSpawnPoint;
    public Vector3 orangeSpawnPoint;

    private void Start()
    {
        Instantiate(ghostRedPrefab, redSpawnPoint, Quaternion.identity);
        Instantiate(ghostPinkPrefab, pinkSpawnPoint, Quaternion.identity);
        Instantiate(ghostCyanPrefab, cyanSpawnPoint, Quaternion.identity);
        Instantiate(ghostOrangePrefab, orangeSpawnPoint, Quaternion.identity);
    }
}
