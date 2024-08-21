using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinkController : MonoBehaviour
{
    public float Sleep = 300f;
    public float speed = 6f;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (Sleep > 0)
        {
            Sleep -= Time.deltaTime;
            return;
        }
        if (inSpawnArea())
            leaveSpawnArea();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "PacMan")
        {
            if (collision.gameObject.GetComponent<PlayerController>().isInvincible)
            {
                Sleep = 10;
                transform.position = new Vector3(0.5f, 0.5f, 0);
            }
            else
            {
                collision.gameObject.SetActive(false);
            }
        }
    }

    private bool inSpawnArea()
    {
        return transform.position.x > -3.5f && transform.position.x < 4.5f && transform.position.y > -1.5 && transform.position.y < 2.5;
    }

    private void leaveSpawnArea()
    {
        if (transform.position.x > 0.45f && transform.position.x < 0.55f)
            rb.velocity = speed * Time.deltaTime * Vector2.up;
        else if (transform.position.x > 0.5f)
            rb.velocity = speed * Time.deltaTime * Vector2.left;
        else
            rb.velocity = speed * Time.deltaTime * Vector2.right;
    }
}
