using UnityEngine;

public class PowerUpCollision : MonoBehaviour
{
    public PowerUp powerUp;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == "PacMan" && collider.GetComponent<PlayerController>().powerUp == null)
        {
            collider.GetComponent<PlayerController>().powerUp = powerUp;
            collider.GetComponent <PlayerController>().sprite = gameObject.GetComponent<SpriteRenderer>().sprite;
            powerUp.onPickUp(gameObject);
        }
    }
}
