using UnityEngine;

public class TunnelController : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("PacMan"))
        {
            TeleportToTunnel();
        }
    }

    private void TeleportToTunnel()
    {
        if (transform.rotation.z == 0)
        {
            RaycastHit2D[] Hits = Physics2D.RaycastAll(transform.position, Vector2.right);
            foreach(RaycastHit2D hit in Hits)
            {
                if (hit.collider.CompareTag("Tunnel"))
                {
                    // Teleport player
                    GameObject.FindGameObjectWithTag("PacMan").transform.position = hit.transform.position;
                }
            }
        }

        if (transform.rotation.z == -1)
        {
            RaycastHit2D[] Hits = Physics2D.RaycastAll(transform.position, Vector2.left);
            foreach (RaycastHit2D hit in Hits)
            {
                if (hit.collider.CompareTag("Tunnel"))
                {
                    // Teleport player
                    GameObject.FindGameObjectWithTag("PacMan").transform.position = hit.transform.position;
                }
            }
        }
    }

}
