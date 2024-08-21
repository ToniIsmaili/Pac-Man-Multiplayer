using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class RedController : MonoBehaviour
{
    public float Sleep = 300f;
    public float speed = 6f;
    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }
    private MapManager mapManager;
    private Rigidbody2D rb2d;
    private float timer = 10f;
    private bool scattering = true;
    private Direction direction = Direction.Right;

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        mapManager = GameObject.Find("MapManager").GetComponent<MapManager>();
    }

    private void Update()
    {
        if (mapManager.map == null) return;
        if (Sleep > 0)
        {
            Sleep -= Time.deltaTime;
            return;
        }
        if (inSpawnArea())
            leaveSpawnArea();
        else if (timer <= 0)
        {
            timer = scattering? 180f : 10f;
            scattering = !scattering;
            direction = TurnBack(direction);
        }
        else
        {
            timer -= Time.deltaTime;
            Direction newDirection = FindPath(PositionToTile(scattering? new Vector2(-15.5f, -16.5f) : GameObject.FindWithTag("PacMan").transform.position));
            if (direction != newDirection && newDirection != TurnBack(direction))
            {
                direction = newDirection;
                Sleep = 0.5f;
            }
        }
        rb2d.velocity = speed * Time.deltaTime * direction.ToVector2();
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
                Destroy(collision.gameObject);
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
            direction = Direction.Up;
        else if (transform.position.x > 0.5f)
            direction = Direction.Left;
        else
            direction = Direction.Right;
    }

    private Direction TurnBack(Direction direction)
    {
        switch (direction)
        {
            case Direction.Up:
                return Direction.Down;
            case Direction.Down:
                return Direction.Up;
            case Direction.Left:
                return Direction.Right;
            case Direction.Right:
                return Direction.Left;
            default:
                return Direction.Right;
        }
    }

    private Vector2 PositionToTile(Vector2 position)
    {
        return new Vector2(Mathf.Round(position.y + 15.5f), Mathf.Round(position.x + 14.5f));
    }

    private Direction FindPath(Vector2 target)
    {
        Debug.Log(scattering + " " + target);
        Vector2 current = PositionToTile(transform.position);
        Direction[] directions = new Direction[] { Direction.Up, Direction.Down, Direction.Left, Direction.Right };
        float minDistance = float.MaxValue;
        Direction bestDirection = direction;
        foreach (Direction dir in directions)
        {
            if (CanMove(dir.ToVector2(), "Wall") && CanMove(dir.ToVector2(), "Barrier") && CanMove(dir.ToVector2(), "Tunnel"))
            {
                Vector2 next = current + dir.ToVector2();
                float distance = Vector2.Distance(next, target);
                if (distance < minDistance && direction != TurnBack(dir))
                {
                    minDistance = distance;
                    bestDirection = dir;
                }
            }
        }
        return bestDirection;
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
}

public static class DirectionExtensions
{
    public static RedController.Direction ToDirection(this Vector2 vector2)
    {
        if (vector2 == Vector2.up)
            return RedController.Direction.Up;
        if (vector2 == Vector2.down)
            return RedController.Direction.Down;
        if (vector2 == Vector2.left)
            return RedController.Direction.Left;
        if (vector2 == Vector2.right)
            return RedController.Direction.Right;
        return RedController.Direction.Right;
    }

    public static Vector2 ToVector2(this RedController.Direction direction)
    {
        switch (direction)
        {
            case RedController.Direction.Up:
                return Vector2.up;
            case RedController.Direction.Down:
                return Vector2.down;
            case RedController.Direction.Left:
                return Vector2.left;
            case RedController.Direction.Right:
                return Vector2.right;
            default:
                return Vector2.right;
        }
    }
}