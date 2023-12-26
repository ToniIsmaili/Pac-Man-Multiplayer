using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            if (collision.gameObject.GetComponent<Testingscript>().isInvincible)
            {
                Destroy(gameObject);
            }
            else
            {
                Destroy(collision.gameObject);
            }
        }

    }
}
