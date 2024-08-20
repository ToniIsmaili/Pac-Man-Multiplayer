using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostController : MonoBehaviour
{
    public float Sleep = 300f;
    public float speed = 6f;

    private void Update()
    {
        if (Sleep > 0)
        {
            Sleep -= Time.deltaTime;
            return;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            if (collision.gameObject.GetComponent<PlayerController>().isInvincible)
            {
                Sleep = 10;
                transform.position = new Vector3(0.5f, 0.5f, 0);
            }
            else
            {
                Destroy(collision.gameObject);
            }
        }
    }
}
