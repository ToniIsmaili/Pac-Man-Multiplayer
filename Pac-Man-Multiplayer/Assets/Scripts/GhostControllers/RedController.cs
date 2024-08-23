using UnityEngine;

public class RedController : MonoBehaviour
{
    public float Sleep = 300f;
    public float speed = 6f;
    public Sprite[] sprites;
    private MapManager mapManager;
    private Rigidbody2D rb2d;
    private float timer = 10f;
    private bool scattering = true;
    private Direction direction = Direction.Right;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        mapManager = GameObject.Find("MapManager").GetComponent<MapManager>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (mapManager.map == null || GameObject.FindWithTag("PacMan") == null) return;
        if (Sleep > 0 || GameObject.FindWithTag("PacMan") == null)
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
            direction = direction.TurnBack();
        }
        else
        {
            timer -= Time.deltaTime;
            Direction newDirection = FindPath(scattering? new Vector2(-15.5f, -16.5f) : GameObject.FindWithTag("PacMan").transform.position);
            if (direction != newDirection && newDirection != direction.TurnBack())
                direction = newDirection;
        }
        if (direction == Direction.Up)
            spriteRenderer.sprite = sprites[0];
        else if (direction == Direction.Down)
            spriteRenderer.sprite = sprites[1];
        else if (direction == Direction.Left)
            spriteRenderer.sprite = sprites[2];
        else
            spriteRenderer.sprite = sprites[3];
        rb2d.velocity = speed * Time.deltaTime * direction.ToVector2();
        transform.rotation = Quaternion.Euler(0, 0, 0);
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

    private Direction FindPath(Vector2 target)
    {
        Vector2 current = transform.position;
        Direction[] directions = new Direction[] { Direction.Up, Direction.Down, Direction.Left, Direction.Right };
        float minDistance = float.MaxValue;
        Direction bestDirection = direction;
        foreach (Direction dir in directions)
        {
            if (CanMove(dir.ToVector2(), "Wall") && CanMove(dir.ToVector2(), "Barrier") && CanMove(dir.ToVector2(), "Tunnel") && CanMove(dir.ToVector2(), "Enemy"))
            {
                Vector2 next = current + dir.ToVector2();
                float distance = Vector2.Distance(next, target);
                if (distance < minDistance && direction != dir.TurnBack())
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
        else if (collision.gameObject.tag == "Enemy")
            direction = direction.TurnBack();
    }
}