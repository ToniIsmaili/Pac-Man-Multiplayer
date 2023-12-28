using UnityEngine;

public class PowerUpCollision : MonoBehaviour
{
    public PowerUp powerUp;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.name == "Player" && collider.GetComponent<PlayerController>().powerUp == null)
        {
            collider.GetComponent<PlayerController>().powerUp = powerUp;
            powerUp.onPickUp(gameObject);
        }
    }
}
