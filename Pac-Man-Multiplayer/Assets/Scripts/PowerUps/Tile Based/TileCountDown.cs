using System.Collections;
using UnityEngine;

public class TileCountDown : MonoBehaviour
{
    public PowerUp powerUp;

    public float tile_duration = 15f;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.name == "Player")
        {
            powerUp.Apply(collider.gameObject);
        }
    }

    void Start()
    {
        // Destroy tile after some time 
        StartTileDuration(gameObject, tile_duration);
    }

    public void StartTileDuration(GameObject target, float duration)
    {
        StartCoroutine(WaitTile(target, duration));
    }

    private IEnumerator WaitTile(GameObject target, float duration)
    {
        // Wait for the specified duration
        yield return new WaitForSeconds(duration);

        // Reset back to its original value
        DeSpawnTile(target);
    }

    private void DeSpawnTile(GameObject target)
    {
        MonoBehaviour.Destroy(target);
    }
}
