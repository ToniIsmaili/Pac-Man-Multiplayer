using UnityEngine;

public class PowerUpCollision : MonoBehaviour
{
    public PowerUp powerUp;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.name == "Player" && collider.GetComponent<Inventory>().powerUp == null)
        {
            collider.GetComponent<Inventory>().powerUp = powerUp;
            Destroy(gameObject);
        }

        // powerUp.Apply(collider.gameObject);
    }
}
