using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpCollision : MonoBehaviour
{

    public PowerUp powerUp;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        Destroy(gameObject);
        powerUp.Apply(collider.gameObject);
    }
}
