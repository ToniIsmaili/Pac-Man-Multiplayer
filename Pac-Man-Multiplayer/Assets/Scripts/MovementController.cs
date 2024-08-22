using UnityEngine;

public class MovementController : MonoBehaviour
{
    [HideInInspector]
    public Vector2 direction = Vector2.zero;
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
        rb.velocity = speed * Time.deltaTime * direction;
    }

    // Checks if the way that the player wants to go is free, and when it is it starts walking there
    private void MovePlayer()
    {
        if (player_input == Vector2.left && CanMove(Vector2.left, "Wall") && CanMove(Vector2.left, "Barrier"))
        {
            direction = player_input;
            transform.localScale = new Vector3(-1, 1, 1);
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        if (player_input == Vector2.right && CanMove(Vector2.right, "Wall") && CanMove(Vector2.right, "Barrier"))
        {
            direction = player_input;
            transform.localScale = new Vector3(1, 1, 1);
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        if (player_input == Vector2.up && CanMove(Vector2.up, "Wall") && CanMove(Vector2.up, "Barrier"))
        {
            direction = player_input;
            transform.localScale = new Vector3(1, 1, 1);
            transform.rotation = Quaternion.Euler(0, 0, 90);
        }
        if (player_input == Vector2.down && CanMove(Vector2.down, "Wall") && CanMove(Vector2.down, "Barrier"))
        {
            direction = player_input;
            transform.localScale = new Vector3(1, 1, 1);
            transform.rotation = Quaternion.Euler(0, 0, -90);
        }
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

    // Checks if there is something in the way
    private bool CanMove(Vector2 direction, string tag)
    {
        RaycastHit2D hit = Physics2D.BoxCast(transform.position, GetComponent<BoxCollider2D>().size, 0, direction, 0.1f);
        // Checks if raycast has collided with anything
        if (hit.collider == null)
            return true;

        // Returns true if it hasnt collided with "tag"
        return !hit.collider.CompareTag(tag);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position + new Vector3(player_input.x * 0.1f, player_input.y * 0.1f, 0), GetComponent<BoxCollider2D>().size);
    }

}
