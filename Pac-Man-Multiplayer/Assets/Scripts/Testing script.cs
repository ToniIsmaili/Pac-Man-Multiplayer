using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testingscript : MonoBehaviour
{
    public float speed = 5f;
    private Rigidbody2D rb;
    private Vector2 player_input;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        player_input.x = Input.GetAxisRaw("Horizontal");
        player_input.y = Input.GetAxisRaw("Vertical");

        player_input.Normalize();
    }

    private void FixedUpdate()
    {
        rb.velocity = speed * player_input * Time.deltaTime;
    }
}
