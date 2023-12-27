using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacDot : MonoBehaviour
{
    private GameManager gameManager = null;

    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == null)
            return;

        if (collision.tag == "PacMan")
        {
            Destroy(gameObject);
            collision.GetComponent<Testingscript>().score++;
            gameManager.dots_remaining--;
        }

    }
}