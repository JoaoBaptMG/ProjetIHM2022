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
    bool grounded;

    Rect boundsRect;

    // Start is called before the first frame update
    void Start()
    {
        velocity = Vector2.zero;
        grounded = false;

        var bounds = GetComponent<SpriteRenderer>().bounds;
        boundsRect = new Rect(bounds.min.x, bounds.min.y, bounds.size.x, bounds.size.y);
    }

    // Integrate using semi-implicit Euler
    void FixedUpdate()
    {
        RespondToGravityAndCollision();
    }

    void Update()
    {
        // Get all the inputs necessary
        ListenToHorizontalInput();
        ListenToJumpInput();
    }

    void ListenToHorizontalInput()
    {
        // First, set up the final speed
        float targetHorSpeed = maxMoveSpeed * Input.GetAxisRaw("Horizontal");

        if (velocity.x < targetHorSpeed)
            velocity.x = Mathf.Min(velocity.x + moveAcceleration * Time.deltaTime, targetHorSpeed);
        else if (velocity.x > targetHorSpeed)
            velocity.x = Mathf.Max(velocity.x - moveAcceleration * Time.deltaTime, targetHorSpeed);
    }

    void ListenToJumpInput()
    {
        // Get the jump input
        if (grounded)
        {
            if (Input.GetButtonDown("Jump"))
            {
                velocity.y = JumpSpeed;
                grounded = false;
            }
        }
        else if (Input.GetButtonUp("Jump"))
            velocity.y = Mathf.Min(velocity.y, JumpReleaseSpeed);
    }

    private void RespondToGravityAndCollision()
    {
        // Build the acceleration vector
        velocity.y -= gravity * Time.fixedDeltaTime;

        // Get movement resolution from the tilemap
        var curRect = new Rect(boundsRect.position + Position, boundsRect.size);
        var resolution = tilemap.MovementSimulation(curRect, velocity, true);
        Position += velocity * Time.fixedDeltaTime + resolution;

        // Check resolution and update velocities accordingly
        if (resolution.x != 0) velocity.x = 0;
        if (resolution.y > 0)
        {
            grounded = true;
            velocity.y = 0;
        }
        else if (resolution.y == 0) grounded = false;
        if (!grounded && resolution.y < 0) velocity.y = 0;
    }
}
