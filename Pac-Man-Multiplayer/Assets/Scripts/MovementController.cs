using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    private Vector2 direction = Vector2.zero;
    private Vector2 player_input = Vector2.zero;
    private Rigidbody2D rb;
    public float speed = 5f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        UserInput();
        MovePlayer();
    }

    private void FixedUpdate()
    {
        rb.velocity = speed * direction * Time.deltaTime;
    }

    // Checks if the way that the player wants to go is free, and when it is it starts walking there
    private void MovePlayer()
    {
        if (player_input == Vector2.left &&
            !Physics2D.BoxCast(transform.position, GetComponent<BoxCollider2D>().size, 0, Vector2.left, 0.1f))
            direction = player_input;
        if (player_input == Vector2.right &&
            !Physics2D.BoxCast(transform.position, GetComponent<BoxCollider2D>().size, 0, Vector2.right, 0.1f))
            direction = player_input;
        if (player_input == Vector2.up &&
            !Physics2D.BoxCast(transform.position, GetComponent<BoxCollider2D>().size, 0, Vector2.up, 0.1f))
            direction = player_input;
        if (player_input == Vector2.down &&
            !Physics2D.BoxCast(transform.position, GetComponent<BoxCollider2D>().size, 0, Vector2.down, 0.1f))
            direction = player_input;
    }

    // Gets the user input and changes the direction of the player
    private void UserInput()
    {
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            player_input = Vector2.left;

        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            player_input = Vector2.right;

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            player_input = Vector2.up;

        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            player_input = Vector2.down;
    }
}
