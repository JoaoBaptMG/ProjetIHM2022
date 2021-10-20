using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Editable properties
    [Header("Movement")]
    public float maxMoveSpeed = 4f;
    public float moveAcceleration = 15f;
    public float turningDeceleration = 30f;
    public float stoppingDeceleration = 22f;
    [Tooltip("If on, the joystick axis will be treated with only three target values, left, neutral or right.")]
    public bool discreteMovement = false;

    [Header("Jump")]
    public float gravity = 2f;
    public float jumpHeight = 4f;
    public float jumpReleaseHeight = 2f;
    [Tooltip("The maximum time in seconds after losing ground until the jump button can be pressed to jump.")]
    public float postLedgeJumpDelay = 0.25f;
    [Tooltip("The maximum time in seconds before connecting with ground where a jump will be registered as valid.")]
    public float jumpBounceDelay = 0.25f;
    public int maxNumJumps = 2;

    [Header("Sprint")]
    public float maxSprintSpeed = 8f;
    public float sprintAcceleration = 30f;
    public float maxSprintTime = 5f;
    public float sprintCooldownTime = 10f;

    [Header("Dash")]
    public float dashDistance = 25f;
    public float dashDuration = 0.5f;

    [Header("Collision")]
    public MyTilemap tilemap;

    // Physical properties
    float JumpSpeed => Mathf.Sqrt(2f * gravity * jumpHeight);
    float JumpReleaseSpeed => Mathf.Sqrt(2f * gravity * jumpReleaseHeight);

    float DashSpeed => dashDistance / dashDuration;

    // Private fields
    Vector2 velocity;
    bool grounded;
    float lastTimeLeftGround;
    float lastTimeJumpPress;
    int numJumps;
    float sprintStartTime;
    float sprintEndTime;
    bool dashed;
    float dashStartTime;

    Rect boundsRect;

    // Transform property
    Vector2 Position
    {
        get { return new Vector2(transform.position.x, transform.position.y); }
        set { transform.position = new Vector3(value.x, value.y, transform.position.z); }
    }

    // Boolean property
    private bool Sprinting => sprintStartTime != float.NegativeInfinity;

    private bool Dashing => dashStartTime != float.NegativeInfinity;

    // Start is called before the first frame update
    void Start()
    {
        velocity = Vector2.zero;
        grounded = false;
        dashed = false;

        var bounds = GetComponent<SpriteRenderer>().bounds;
        boundsRect = new Rect(bounds.min.x, bounds.min.y, bounds.size.x, bounds.size.y);

        // Set timer variables
        lastTimeLeftGround = float.NegativeInfinity;
        lastTimeJumpPress = float.NegativeInfinity;
        sprintStartTime = float.NegativeInfinity;
        sprintEndTime = float.NegativeInfinity;
        dashStartTime = float.NegativeInfinity;

        // And counters
        numJumps = 0;
    }

    // Integrate using semi-implicit Euler
    void FixedUpdate()
    {
        RespondToGravityAndCollision();
    }

    void Update()
    {
        // Get all the inputs necessary
        HandleSprinting();
        HandleHorizontalMovement();
        HandleDash();
        HandleJump();
    }

    // Compute the discrete movement
    private float DiscreteHorizontalMovement
    {
        get
        {
            var input = Input.GetAxisRaw("Horizontal");
            if (input <= -0.4f) return -1f;
            if (input >= 0.4f) return 1f;
            return 0f;
        }
    }

    // Get the actual horizontal movement, depending on the discrete movement
    private float HorizontalMovement => discreteMovement ? DiscreteHorizontalMovement : Input.GetAxisRaw("Horizontal");

    void HandleHorizontalMovement()
    {
        // First, set up the final speed
        float maxStateSpeed = Sprinting ? maxSprintSpeed : maxMoveSpeed;
        float targetHorSpeed = maxStateSpeed * HorizontalMovement;
        bool changingSpeed = targetHorSpeed * velocity.x < 0;

        // Compute the acceleration
        float acceleration;
        // Consider the stopping and turning cases
        if (Sprinting) acceleration = sprintAcceleration;
        else if (Mathf.Abs(targetHorSpeed) < 0.0625f)
            acceleration = stoppingDeceleration;
        else if (changingSpeed) // Which means they have opposite signs
            acceleration = turningDeceleration;
        else acceleration = moveAcceleration;

        // And get the delta velocity from it
        float deltaVelocity = acceleration * Time.deltaTime;
        if (velocity.x < targetHorSpeed)
            velocity.x = Mathf.Min(velocity.x + deltaVelocity, targetHorSpeed);
        else if (velocity.x > targetHorSpeed)
            velocity.x = Mathf.Max(velocity.x - deltaVelocity, targetHorSpeed);
    }

    void HandleSprinting()
    {
        // Get delta time here
        var now = Time.time;
        bool sprintButton = Input.GetButton("SprintDash");
        // Handle sprinting
        if (Sprinting)
        {
            // Stop sprint when leaving ground
            if (!sprintButton || !grounded)
            {
                // Set a proportional cooldown time for the sprint
                float factor = (now - sprintStartTime) / maxSprintTime;
                sprintEndTime = now - (1.0f - factor) * sprintCooldownTime;

                sprintStartTime = float.NegativeInfinity;
                EndSprint(exhaustion: false);
            }
            else if (now - sprintStartTime >= maxSprintTime) // Exhaustion
            {
                sprintStartTime = float.NegativeInfinity;
                sprintEndTime = now;
                EndSprint(exhaustion: true);
            }
        }
        else if (now - sprintEndTime > sprintCooldownTime)
        {
            sprintEndTime = float.NegativeInfinity;
            if (sprintButton && grounded)
            {
                BeginSprint();
                sprintStartTime = now;
            }
        }
    }

    void BeginSprint()
    {
        // TODO: Modify here for possible particles or changes
        Debug.Log(nameof(BeginSprint));
    }

    void EndSprint(bool exhaustion)
    {
        // TODO: Modify here for possible particles or changes
        Debug.Log(nameof(EndSprint) + ": " + (exhaustion ? "exhaustion" : ""));
    }

    void HandleDash()
    {
        var now = Time.time;

        // Check if the player is airborne and the dash jump is pressed
        if (!grounded && !dashed && Input.GetButtonDown("SprintDash"))
        {
            BeginDash();
            dashed = true;
            dashStartTime = now;
        }

        if (now - dashStartTime < dashDuration)
        {
            velocity = new Vector2(Mathf.Sign(velocity.x) * DashSpeed, 0f);
        }
        else
        {
            if (Dashing)
            {
                velocity.x = Mathf.Sign(velocity.x) * maxMoveSpeed;
                EndDash();
            }
            dashStartTime = float.NegativeInfinity;
        }
    }

    void BeginDash()
    {
        // TODO: Modify here for possible particles or changes
        Debug.Log(nameof(BeginDash));
    }

    void EndDash()
    {
        // TODO: Modify here for possible particles or changes
        Debug.Log(nameof(EndDash));
    }

    void HandleJump()
    {
        // Get delta time here
        var now = Time.time;

        // Listen to the jump button
        if (Input.GetButtonDown("Jump"))
            lastTimeJumpPress = now;
        bool jumpPress = now - lastTimeJumpPress <= jumpBounceDelay;

        // Check if the user just left ground
        bool shortAfterFall = now - lastTimeLeftGround <= postLedgeJumpDelay;

        if (grounded || shortAfterFall || numJumps < maxNumJumps)
        {
            if (jumpPress)
            {
                // Reset the jump press variable
                lastTimeJumpPress = float.NegativeInfinity;
                JumpAction();
            }
        }
        else if (Input.GetButtonUp("Jump"))
            velocity.y = Mathf.Min(velocity.y, JumpReleaseSpeed);
    }

    private void JumpAction()
    {
        // Set relevant variables
        velocity.y = JumpSpeed;
        grounded = false;
        numJumps++;
    }

    private void RespondToGravityAndCollision()
    {
        // Build the acceleration vector
        velocity.y -= gravity * Time.fixedDeltaTime;

        // Get movement resolution from the tilemap
        var deltaPosition = velocity * Time.fixedDeltaTime;
        var curRect = new Rect(boundsRect.position + Position, boundsRect.size);
        var resolution = tilemap.MovementSimulation(curRect, deltaPosition);
        Position += deltaPosition + resolution;

        // Check resolution and update velocities accordingly
        // Check and update grounded state as well
        if (resolution.x != 0)
        {
            velocity.x = 0;
            // Stop dashing
            dashStartTime = Time.fixedTime - dashDuration;
        }
        if (resolution.y > 0)
        {
            if (!grounded) Ground();
            velocity.y = 0;
            grounded = true;
        }
        else if (resolution.y == 0)
        {
            if (grounded) BeginFall();
            grounded = false;
        }
        if (!grounded && resolution.y < 0) velocity.y = 0;
    }

    private void BeginFall()
    {
        // Set all important variables when losing ground
        lastTimeLeftGround = Time.fixedTime;
    }

    // Action 
    private void Ground()
    {
        numJumps = 0;
        dashed = false;
    }
}
