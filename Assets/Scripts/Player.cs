using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Editable properties
    public float gravity = 2f;
    public float moveAcceleration = 1f;
    public float maxMoveSpeed = 4f;
    public float jumpHeight = 4f;
    public float jumpReleaseHeight = 2f;

    // Connections
    public MyTilemap tilemap;

    // Properties
    float JumpSpeed => Mathf.Sqrt(2f * gravity * jumpHeight);
    float JumpReleaseSpeed => Mathf.Sqrt(2f * gravity * jumpReleaseHeight);

    // Transform property
    Vector2 Position
    {
        get { return new Vector2(transform.position.x, transform.position.y); }
        set { transform.position = new Vector3(value.x, value.y, transform.position.z); }
    }

    // Private fields
    Vector2 velocity;
    float horAccel;
    bool grounded;

    // Start is called before the first frame update
    void Start()
    {
        velocity = Vector2.zero;
        horAccel = 0;
        grounded = false;
    }

    // Integrate using semi-implicit Euler
    void FixedUpdate()
    {
        // Build the acceleration vector
        var acceleration = new Vector2(horAccel, grounded ? 0f : -gravity);
        velocity += acceleration * Time.fixedDeltaTime;
        Position += velocity * Time.fixedDeltaTime;
    }

    // Collision for now
    void Update()
    {
        ManageTilemapCollision();
    }

    void ManageTilemapCollision()
    {
        // Check collision with the ground
        float x = Position.x, y = Position.y;
        var tile = tilemap[x, y - 0.5f];
        var pos = Position;

        if (tile != null)
        {
            grounded = true;
            pos.y = tile.transform.position.y + 1f;
            velocity.y = 0f;
        }
        else grounded = false;

        Position = pos;
    }
}
