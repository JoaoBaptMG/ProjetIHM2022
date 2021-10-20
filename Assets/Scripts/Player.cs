using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Adjustable properties
    [SerializeField]
    private float maximumWalkingSpeed;
    [SerializeField]
    private float maximumRunningSpeed;
    [SerializeField]
    private float midAirMaximumHorizontalSpeed;
    [SerializeField]
    private float maximumDashingSpeed;
    [SerializeField]
    private float maximumFreeFallSpeed;
    [SerializeField]
    private float maximumWallSlideSpeed;
    [SerializeField]
    private float jumpSpeed;
    [SerializeField]
    private int maximumNumberOfJumps;

    public float MaximumWalkingSpeed { get { return maximumWalkingSpeed; } }
    public float MaximumRunningSpeed { get { return maximumRunningSpeed; } }
    public float MidAirMaximumHorizontalSpeed { get { return midAirMaximumHorizontalSpeed; } }
    public float MaximumDashingSpeed { get { return maximumDashingSpeed; } }
    public float MaximumFreeFallSpeed { get { return maximumFreeFallSpeed; } }
    public float MaximumWallSlideSpeed { get { return maximumWallSlideSpeed; } }
    public float JumpSpeed { get { return jumpSpeed; } }
    public int MaximumNumberOfJumps { get { return maximumNumberOfJumps; } }
    // End of adjustable properties

    // The transition factory to provide transitions.
    private TransitionFactory transitionFactory;

    // The transitions needed for the player.
    private Transition horizontalSpeedTransition;
    private Transition verticalSpeedTransition;

    // Physics
    private Vector3 dimensions;
    private Vector3 acceleration;
    private Vector3 speed;

    // Either 1 or -1.
    // 1 when the player is facing right, -1 when they are facing left.
    public int Orientation { private get; set; }

    private int numberOfJumpsLeft;

    public bool Grounded { get; set; }

    // Statuses
    public PlayerStatus PreviousStatus { get; private set; }
    public PlayerStatus CurrentStatus { get; private set; }

    // To be called whenever the player's status can potentially change.
    public void updateStatus(PlayerStatus newStatus)
    {
        // If the parameter given already is the player's current status, 
        // no status update is required.
        if(newStatus == CurrentStatus) { return; }

        // Updates the status stack.
        PreviousStatus = CurrentStatus;
        CurrentStatus = newStatus;

        // Cancels all transitions currently referenced by the player 
        // and marks them as available for later use.
        if(horizontalSpeedTransition != null)
        {
            horizontalSpeedTransition.Available = true;
            horizontalSpeedTransition = null;
        }
        if(verticalSpeedTransition != null)
        {
            verticalSpeedTransition.Available = true;
            verticalSpeedTransition = null;
        }

        // Setting new transitions on the player depending on its previous 
        // state, and its newly aquired current status.
        // (EXTREMELY UGLY HUGE ASS CONDITIONAL SWITCH..... -> State design pattern?)
        switch (PreviousStatus)
        {
            case PlayerStatus.GROUNDED_IDLE:

                switch (CurrentStatus)
                {
                    // PlayerStatus.GROUNDED_IDLE to PlayerStatus.WALK
                    case PlayerStatus.WALK:
                        if(maximumWalkingSpeed > speed.x)
                        {
                            // Accelerating to the maximum walking speed.
                            horizontalSpeedTransition = transitionFactory.getTransition(typeof(EaseInExponentialTransition));
                            horizontalSpeedTransition.Duration = 0.083f; // 5 frames
                        }
                        else
                        {
                            // Decelerating to the maximum walking speed.
                            horizontalSpeedTransition = transitionFactory.getTransition(typeof(EaseOutExponentialTransition));
                            horizontalSpeedTransition.Duration = 0.1f * Mathf.Abs(maximumWalkingSpeed - speed.x);
                        }
                        horizontalSpeedTransition.From = speed.x;
                        horizontalSpeedTransition.To = Orientation * maximumWalkingSpeed;
                        break;

                    // PlayerStatus.GROUNDED_IDLE to PlayerStatus.RUN
                    case PlayerStatus.RUN:
                        if (maximumRunningSpeed > speed.x)
                        {
                            // Accelerating to the maximum running speed.
                            horizontalSpeedTransition = transitionFactory.getTransition(typeof(EaseInExponentialTransition));
                            horizontalSpeedTransition.Duration = 0.17f; // 10 frames.
                        }
                        else
                        {
                            // Decelerating to the maximum running speed
                            horizontalSpeedTransition = transitionFactory.getTransition(typeof(EaseOutExponentialTransition));
                            horizontalSpeedTransition.Duration = 0.1f * Mathf.Abs(maximumRunningSpeed - speed.x);
                        }
                        horizontalSpeedTransition.From = speed.x;
                        horizontalSpeedTransition.To = Orientation * maximumRunningSpeed;
                        break;

                    // PlayerStatus.GROUNDED_IDLE to PlayerStatus.GROUND_SLIDE
                    case PlayerStatus.GROUND_SLIDE:

                        // Decelerating to a horizontal speed of 0.
                        horizontalSpeedTransition = transitionFactory.getTransition(typeof(LinearTransition));
                        horizontalSpeedTransition.Duration = 0.3f * Mathf.Abs(speed.x);
                        horizontalSpeedTransition.From = speed.x;
                        horizontalSpeedTransition.To = 0;
                        break;

                    // PlayerStatus.GROUNDED_IDLE to PlayerStatus.MIDAIR_IDLE
                    case PlayerStatus.MIDAIR_IDLE:

                        // Applies jump speed.
                        speed.y = jumpSpeed;

                        // Decelerating to a horizontal speed of 0.
                        horizontalSpeedTransition = transitionFactory.getTransition(typeof(EaseOutExponentialTransition));
                        horizontalSpeedTransition.Duration = 0.16f; // 10 frames
                        horizontalSpeedTransition.From = speed.x;
                        horizontalSpeedTransition.To = 0;
                        break;

                    // PlayerStatus.GROUNDED_IDLE to PlayerStatus.AIR_CONTROL
                    case PlayerStatus.AIR_CONTROL:
                        break;

                    // PlayerStatus.GROUNDED_IDLE to PlayerStatus.WALL_SLIDE
                    case PlayerStatus.WALL_SLIDE:
                        break;

                    // PlayerStatus.GROUNDED_IDLE to PlayerStatus.DASH
                    case PlayerStatus.DASH:
                        break;
                }

                break;

            case PlayerStatus.WALK:

                switch (CurrentStatus)
                {
                    // PlayerStatus.WALK to PlayerStatus.GROUNDED_IDLE
                    case PlayerStatus.GROUNDED_IDLE:

                        // Decelerating to an horizontal speed of 0.
                        horizontalSpeedTransition = transitionFactory.getTransition(typeof(EaseOutExponentialTransition));
                        horizontalSpeedTransition.Duration = 0.09f * Mathf.Abs(speed.x);
                        horizontalSpeedTransition.From = speed.x;
                        horizontalSpeedTransition.To = 0;
                        break;

                    // PlayerStatus.WALK to PlayerStatus.RUN
                    case PlayerStatus.RUN:
                        if (maximumRunningSpeed > speed.x)
                        {
                            // Accelerating to the maximum running speed.
                            horizontalSpeedTransition = transitionFactory.getTransition(typeof(EaseInExponentialTransition));
                            horizontalSpeedTransition.Duration = 0.12f; // 7 frames
                        }
                        else
                        {
                            // Decelerating to the maximum running speed
                            horizontalSpeedTransition = transitionFactory.getTransition(typeof(EaseOutExponentialTransition));
                            horizontalSpeedTransition.Duration = 0.1f * Mathf.Abs(maximumRunningSpeed - speed.x);
                        }
                        horizontalSpeedTransition.From = speed.x;
                        horizontalSpeedTransition.To = Orientation * maximumRunningSpeed;
                        break;

                    // PlayerStatus.WALK to PlayerStatus.GROUND_SLIDE
                    case PlayerStatus.GROUND_SLIDE:

                        // Decelerating to a horizontal speed of 0.
                        horizontalSpeedTransition = transitionFactory.getTransition(typeof(LinearTransition));
                        horizontalSpeedTransition.Duration = 0.3f * Mathf.Abs(speed.x);
                        horizontalSpeedTransition.From = speed.x;
                        horizontalSpeedTransition.To = 0;
                        break;

                    // PlayerStatus.WALK to PlayerStatus.MIDAIR_IDLE
                    case PlayerStatus.MIDAIR_IDLE:

                        // Applies jump speed.
                        speed.y = jumpSpeed;

                        // Decelerating to a horizontal speed of 0.
                        horizontalSpeedTransition = transitionFactory.getTransition(typeof(EaseOutExponentialTransition));
                        horizontalSpeedTransition.Duration = 0.16f; // 10 frames
                        horizontalSpeedTransition.From = speed.x;
                        horizontalSpeedTransition.To = 0;
                        break;

                    // PlayerStatus.WALK to PlayerStatus.AIR_CONTROL
                    case PlayerStatus.AIR_CONTROL:
                        break;

                    // PlayerStatus.WALK to PlayerStatus.WALL_SLIDE
                    case PlayerStatus.WALL_SLIDE:
                        break;

                    // PlayerStatus.WALK to PlayerStatus.DASH
                    case PlayerStatus.DASH:
                        break;
                }

                break;

            case PlayerStatus.RUN:

                switch (CurrentStatus)
                {
                    // PlayerStatus.RUN to PlayerStatus.GROUNDED_IDLE
                    case PlayerStatus.GROUNDED_IDLE:

                        // Decelerating to an horizontal speed of 0.
                        horizontalSpeedTransition = transitionFactory.getTransition(typeof(EaseOutExponentialTransition));
                        horizontalSpeedTransition.Duration = 0.09f * Mathf.Abs(speed.x);
                        horizontalSpeedTransition.From = speed.x;
                        horizontalSpeedTransition.To = 0;
                        break;

                    // PlayerStatus.RUN to PlayerStatus.WALK
                    case PlayerStatus.WALK:
                        if (maximumWalkingSpeed > speed.x)
                        {
                            // Accelerating to the maximum walking speed.
                            horizontalSpeedTransition = transitionFactory.getTransition(typeof(EaseInExponentialTransition));
                            horizontalSpeedTransition.Duration = 0.067f; // 4 frames
                        }
                        else
                        {
                            // Decelerating to the maximum walking speed
                            horizontalSpeedTransition = transitionFactory.getTransition(typeof(EaseOutExponentialTransition));
                            horizontalSpeedTransition.Duration = 0.1f * Mathf.Abs(maximumWalkingSpeed - speed.x);
                        }
                        horizontalSpeedTransition.From = speed.x;
                        horizontalSpeedTransition.To = Orientation * maximumWalkingSpeed;
                        break;

                    // PlayerStatus.RUN to PlayerStatus.GROUND_SLIDE
                    case PlayerStatus.GROUND_SLIDE:

                        // Decelerating to a horizontal speed of 0.
                        horizontalSpeedTransition = transitionFactory.getTransition(typeof(LinearTransition));
                        horizontalSpeedTransition.Duration = 0.3f * Mathf.Abs(speed.x);
                        horizontalSpeedTransition.From = speed.x;
                        horizontalSpeedTransition.To = 0;
                        break;

                    // PlayerStatus.RUN to PlayerStatus.MIDAIR_IDLE
                    case PlayerStatus.MIDAIR_IDLE:

                        // Applies jump speed.
                        speed.y = jumpSpeed;

                        // Decelerating to a horizontal speed of 0.
                        horizontalSpeedTransition = transitionFactory.getTransition(typeof(EaseOutExponentialTransition));
                        horizontalSpeedTransition.Duration = 0.16f; // 10 frames
                        horizontalSpeedTransition.From = speed.x;
                        horizontalSpeedTransition.To = 0;
                        break;

                    // PlayerStatus.RUN to PlayerStatus.AIR_CONTROL
                    case PlayerStatus.AIR_CONTROL:
                        break;

                    // PlayerStatus.RUN to PlayerStatus.WALL_SLIDE
                    case PlayerStatus.WALL_SLIDE:
                        break;

                    // PlayerStatus.RUN to PlayerStatus.DASH
                    case PlayerStatus.DASH:
                        break;
                }

                break;

            case PlayerStatus.GROUND_SLIDE:

                switch (CurrentStatus)
                {
                    // PlayerStatus.GROUND_SLIDE to PlayerStatus.GROUNDED_IDLE
                    case PlayerStatus.GROUNDED_IDLE:

                        // Decelerating to an horizontal speed of 0.
                        horizontalSpeedTransition = transitionFactory.getTransition(typeof(EaseOutExponentialTransition));
                        horizontalSpeedTransition.Duration = 0.09f * Mathf.Abs(speed.x);
                        horizontalSpeedTransition.From = speed.x;
                        horizontalSpeedTransition.To = 0;
                        break;

                    // PlayerStatus.GROUND_SLIDE to PlayerStatus.WALK
                    case PlayerStatus.WALK:
                        horizontalSpeedTransition = transitionFactory.getTransition(typeof(EaseInExponentialTransition));
                        horizontalSpeedTransition.Duration = 0.1f * Mathf.Abs(maximumWalkingSpeed - speed.x);
                        horizontalSpeedTransition.From = speed.x;
                        horizontalSpeedTransition.To = Orientation * maximumWalkingSpeed;
                        break;

                    // PlayerStatus.GROUND_SLIDE to PlayerStatus.RUN
                    case PlayerStatus.RUN:
                        horizontalSpeedTransition = transitionFactory.getTransition(typeof(EaseInExponentialTransition));
                        horizontalSpeedTransition.Duration = 0.1f * Mathf.Abs(maximumRunningSpeed - speed.x);
                        horizontalSpeedTransition.From = speed.x;
                        horizontalSpeedTransition.To = Orientation * maximumRunningSpeed;
                        break;

                    // PlayerStatus.GROUND_SLIDE to PlayerStatus.MIDAIR_IDLE
                    case PlayerStatus.MIDAIR_IDLE:

                        // Applies jump speed.
                        speed.y = jumpSpeed;

                        // Decelerating to a horizontal speed of 0.
                        horizontalSpeedTransition = transitionFactory.getTransition(typeof(EaseOutExponentialTransition));
                        horizontalSpeedTransition.Duration = 0.16f; // 10 frames
                        horizontalSpeedTransition.From = speed.x;
                        horizontalSpeedTransition.To = 0;
                        break;

                    // PlayerStatus.GROUND_SLIDE to PlayerStatus.AIR_CONTROL
                    case PlayerStatus.AIR_CONTROL:
                        break;

                    // PlayerStatus.GROUND_SLIDE to PlayerStatus.WALL_SLIDE
                    case PlayerStatus.WALL_SLIDE:
                        break;

                    // PlayerStatus.GROUND_SLIDE to PlayerStatus.DASH
                    case PlayerStatus.DASH:
                        break;
                }

                break;

            case PlayerStatus.MIDAIR_IDLE:

                switch (CurrentStatus)
                {
                    // PlayerStatus.MIDAIR_IDLE to PlayerStatus.GROUNDED_IDLE
                    case PlayerStatus.GROUNDED_IDLE:

                        // Cancelling vertical speed.
                        speed.y = 0;

                        // Decreasing horizontal speed to 0.
                        horizontalSpeedTransition = transitionFactory.getTransition(typeof(EaseInOutQuartTransition));
                        horizontalSpeedTransition.Duration = 0.1f * Mathf.Abs(speed.x);
                        horizontalSpeedTransition.From = speed.x;
                        horizontalSpeedTransition.To = 0;
                        break;

                    // PlayerStatus.MIDAIR_IDLE to PlayerStatus.WALK
                    case PlayerStatus.WALK:
                        break;

                    // PlayerStatus.MIDAIR_IDLE to PlayerStatus.RUN
                    case PlayerStatus.RUN:
                        break;

                    // PlayerStatus.MIDAIR_IDLE to PlayerStatus.GROUND_SLIDE
                    case PlayerStatus.GROUND_SLIDE:
                        break;

                    // PlayerStatus.MIDAIR_IDLE to PlayerStatus.AIR_CONTROL
                    case PlayerStatus.AIR_CONTROL:
                        horizontalSpeedTransition = transitionFactory.getTransition(typeof(EaseOutExponentialTransition));
                        horizontalSpeedTransition.Duration = 0.12f; // 7 frames
                        horizontalSpeedTransition.From = speed.x;
                        horizontalSpeedTransition.To = Orientation * MidAirMaximumHorizontalSpeed;
                        break;

                    // PlayerStatus.MIDAIR_IDLE to PlayerStatus.WALL_SLIDE
                    case PlayerStatus.WALL_SLIDE:

                        // Cancels horizontal speed.
                        speed.x = 0;

                        // Set wall-slide speed transition
                        verticalSpeedTransition = transitionFactory.getTransition(typeof(EaseInExponentialTransition));
                        verticalSpeedTransition.Duration = 0.42f; // 25 frames
                        verticalSpeedTransition.From = 0;
                        verticalSpeedTransition.To = -maximumWallSlideSpeed;
                        break;

                    // PlayerStatus.MIDAIR_IDLE to PlayerStatus.DASH
                    case PlayerStatus.DASH:
                        speed.x = Orientation * MaximumDashingSpeed;
                        break;
                }

                break;

            case PlayerStatus.AIR_CONTROL:

                switch (CurrentStatus)
                {
                    // PlayerStatus.AIR_CONTROL to PlayerStatus.GROUNDED_IDLE
                    case PlayerStatus.GROUNDED_IDLE:

                        // Cancelling vertical speed.
                        speed.y = 0;

                        // Decreasing horizontal speed to 0.
                        horizontalSpeedTransition = transitionFactory.getTransition(typeof(EaseInOutQuartTransition));
                        horizontalSpeedTransition.Duration = 0.1f * Mathf.Abs(speed.x);
                        horizontalSpeedTransition.From = speed.x;
                        horizontalSpeedTransition.To = 0;
                        break;

                    // PlayerStatus.AIR_CONTROL to PlayerStatus.WALK
                    case PlayerStatus.WALK:
                        break;

                    // PlayerStatus.AIR_CONTROL to PlayerStatus.RUN
                    case PlayerStatus.RUN:
                        break;

                    // PlayerStatus.AIR_CONTROL to PlayerStatus.GROUND_SLIDE
                    case PlayerStatus.GROUND_SLIDE:
                        break;

                    // PlayerStatus.AIR_CONTROL to PlayerStatus.MIDAIR_IDLE
                    case PlayerStatus.MIDAIR_IDLE:

                        // Decelerating to a horizontal speed of 0.
                        horizontalSpeedTransition = transitionFactory.getTransition(typeof(EaseOutExponentialTransition));
                        horizontalSpeedTransition.Duration = 0.16f; // 10 frames
                        horizontalSpeedTransition.From = speed.x;
                        horizontalSpeedTransition.To = 0;
                        break;

                    // PlayerStatus.AIR_CONTROL to PlayerStatus.WALL_SLIDE
                    case PlayerStatus.WALL_SLIDE:

                        // Cancels horizontal speed.
                        speed.x = 0;

                        // Set wall-slide speed transition
                        verticalSpeedTransition = transitionFactory.getTransition(typeof(EaseInExponentialTransition));
                        verticalSpeedTransition.Duration = 0.5f; // 30 frames
                        verticalSpeedTransition.From = 0;
                        verticalSpeedTransition.To = -maximumWallSlideSpeed;
                        break;

                    // PlayerStatus.AIR_CONTROL to PlayerStatus.DASH
                    case PlayerStatus.DASH:
                        speed.x = Orientation * MaximumDashingSpeed;
                        break;
                }

                break;

            case PlayerStatus.WALL_SLIDE:

                switch (CurrentStatus)
                {
                    // PlayerStatus.WALL_SLIDE to PlayerStatus.GROUNDED_IDLE
                    case PlayerStatus.GROUNDED_IDLE:

                        // Cancelling vertical speed.
                        speed.y = 0;
                        break;

                    // PlayerStatus.WALL_SLIDE to PlayerStatus.WALK
                    case PlayerStatus.WALK:
                        break;

                    // PlayerStatus.WALL_SLIDE to PlayerStatus.RUN
                    case PlayerStatus.RUN:
                        break;

                    // PlayerStatus.WALL_SLIDE to PlayerStatus.GROUND_SLIDE
                    case PlayerStatus.GROUND_SLIDE:
                        break;

                    // PlayerStatus.WALL_SLIDE to PlayerStatus.MIDAIR_IDLE
                    case PlayerStatus.MIDAIR_IDLE:

                        speed.y = JumpSpeed;
                        speed.x = -Orientation * JumpSpeed * 0.5f;
                        break;

                    // PlayerStatus.WALL_SLIDE to PlayerStatus.AIR_CONTROL
                    case PlayerStatus.AIR_CONTROL:
                        break;

                    // PlayerStatus.WALL_SLIDE to PlayerStatus.DASH
                    case PlayerStatus.DASH:
                        break;
                }

                break;

            case PlayerStatus.DASH:

                switch (CurrentStatus)
                {
                    // PlayerStatus.DASH to PlayerStatus.GROUNDED_IDLE
                    case PlayerStatus.GROUNDED_IDLE:

                        // Cancelling vertical speed.
                        speed.y = 0;

                        // Decreasing horizontal speed to 0.
                        horizontalSpeedTransition = transitionFactory.getTransition(typeof(EaseInOutQuartTransition));
                        horizontalSpeedTransition.Duration = 0.1f * Mathf.Abs(speed.x);
                        horizontalSpeedTransition.From = speed.x;
                        horizontalSpeedTransition.To = 0;
                        break;

                    // PlayerStatus.DASH to PlayerStatus.WALK
                    case PlayerStatus.WALK:
                        break;

                    // PlayerStatus.DASH to PlayerStatus.RUN
                    case PlayerStatus.RUN:
                        break;

                    // PlayerStatus.DASH to PlayerStatus.GROUND_SLIDE
                    case PlayerStatus.GROUND_SLIDE:
                        break;

                    // PlayerStatus.DASH to PlayerStatus.MIDAIR_IDLE
                    case PlayerStatus.MIDAIR_IDLE:

                        // Decelerating to a horizontal speed of 0.
                        horizontalSpeedTransition = transitionFactory.getTransition(typeof(EaseOutExponentialTransition));
                        horizontalSpeedTransition.Duration = 0.33f; // 20 frames
                        horizontalSpeedTransition.From = speed.x;
                        horizontalSpeedTransition.To = 0;
                        break;

                    // PlayerStatus.DASH to PlayerStatus.AIR_CONTROL
                    case PlayerStatus.AIR_CONTROL:
                        horizontalSpeedTransition = transitionFactory.getTransition(typeof(EaseOutExponentialTransition));
                        horizontalSpeedTransition.Duration = 0.24f; // 14 frames
                        horizontalSpeedTransition.From = speed.x;
                        horizontalSpeedTransition.To = Orientation * MidAirMaximumHorizontalSpeed;
                        break;

                    // PlayerStatus.DASH to PlayerStatus.WALL_SLIDE
                    case PlayerStatus.WALL_SLIDE:

                        // Cancels horizontal speed.
                        speed.x = 0;

                        // Set wall-slide speed transition
                        verticalSpeedTransition = transitionFactory.getTransition(typeof(EaseInExponentialTransition));
                        verticalSpeedTransition.Duration = 1f; // 60 frames
                        verticalSpeedTransition.From = 0;
                        verticalSpeedTransition.To = -maximumWallSlideSpeed;
                        break;
                }

                break;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        transitionFactory = TransitionFactory.getInstance();

        GameObject game = GameObject.Find("Game");
        if (!game) { Debug.LogError("No game object nammed 'Game' was found."); Application.Quit(); }

        GameManager gameManager = game.GetComponent<GameManager>();
        if(!gameManager) { Debug.LogError("Game object 'Game' has no component of class GameManager."); Application.Quit(); }

        dimensions = GetComponent<Renderer>().bounds.size;
        acceleration = new Vector3(0, -gameManager.Gravity, 0);
        speed = new Vector3(0, 0, 0);

        numberOfJumpsLeft = maximumNumberOfJumps;
        Orientation = 1;
        Grounded = false;

        PreviousStatus = PlayerStatus.MIDAIR_IDLE;
        CurrentStatus = PlayerStatus.MIDAIR_IDLE;
    }

    // Update is called once per frame
    void Update()
    {
        PlayerStatus status = CurrentStatus;

        // Trying to apply horizontal speed transition.
        if (horizontalSpeedTransition != null)
        {
            if (horizontalSpeedTransition.isFinished())
            {
                horizontalSpeedTransition.Available = true;
                horizontalSpeedTransition = null;
            }
            else
            {
                speed.x = horizontalSpeedTransition.getValue();
            }
        }
        else
        {
            speed.x += acceleration.x * Time.deltaTime;
        }

        // Trying to apply vertical speed transition.
        if (verticalSpeedTransition != null)
        {
            if (verticalSpeedTransition.isFinished())
            {
                verticalSpeedTransition.Available = true;
                verticalSpeedTransition = null;
            }
            else
            {
                speed.y = verticalSpeedTransition.getValue();
            }
        }
        else
        {
            speed.y += acceleration.y * Time.deltaTime;
        }

        // Clamping the vertical speed.
        if (speed.y < -MaximumFreeFallSpeed) { speed.y = -MaximumFreeFallSpeed; }

        // Preemptive collision detection.

        // Updating the coordinates of the corners of the player's bounding box.
        Vector2 upperLeftCorner = new Vector2(transform.position.x - dimensions.x / 2, transform.position.y + dimensions.y / 2);
        Vector2 lowerLeftCorner = new Vector2(transform.position.x - dimensions.x / 2, transform.position.y - dimensions.y / 2);
        Vector2 upperRightCorner = new Vector2(transform.position.x + dimensions.x / 2, transform.position.y + dimensions.y / 2);
        Vector2 lowerRightCorner = new Vector2(transform.position.x + dimensions.x / 2, transform.position.y - dimensions.y / 2);

        RaycastHit2D[] topCornerHorizontalCollisions;
        RaycastHit2D[] bottomCornerHorizontalCollisions;
        RaycastHit2D[] leftCornerVerticalCollisions;
        RaycastHit2D[] rightCornerVerticalCollisions;

        // Checking for collisions by casting lines from the corners of the player's bounding box, 
        // in directions depending on the player's speed direction.
        if (speed.x > 0)
        {
            topCornerHorizontalCollisions = Physics2D.LinecastAll(upperRightCorner, upperRightCorner + new Vector2(Time.deltaTime * speed.x, 0));
            bottomCornerHorizontalCollisions = Physics2D.LinecastAll(lowerRightCorner, lowerRightCorner + new Vector2(Time.deltaTime * speed.x, 0));

            // Collision between the player and a game objet on their right.
            if (topCornerHorizontalCollisions.Length + bottomCornerHorizontalCollisions.Length > 2)
            {
                status = PlayerStatus.WALL_SLIDE;

                if (topCornerHorizontalCollisions.Length > 1 && bottomCornerHorizontalCollisions.Length > 1) 
                {
                    transform.position = new Vector3(Mathf.Min(transform.position.x + topCornerHorizontalCollisions[1].distance, transform.position.x + bottomCornerHorizontalCollisions[1].distance), transform.position.y, 0);
                }
                else if (topCornerHorizontalCollisions.Length > 1 && bottomCornerHorizontalCollisions.Length < 2) 
                {
                    transform.position = new Vector3(transform.position.x + topCornerHorizontalCollisions[1].distance, transform.position.y, 0);
                }
                else 
                {
                    transform.position = new Vector3(transform.position.x + bottomCornerHorizontalCollisions[1].distance, transform.position.y, 0);
                }
            }
        }
        else if (speed.x < 0)
        {
            topCornerHorizontalCollisions = Physics2D.LinecastAll(upperLeftCorner, upperLeftCorner + new Vector2(Time.deltaTime * speed.x, 0));
            bottomCornerHorizontalCollisions = Physics2D.LinecastAll(lowerLeftCorner, lowerLeftCorner + new Vector2(Time.deltaTime * speed.x, 0));

            // Collision between the player and a game objet on their left.
            if (topCornerHorizontalCollisions.Length + bottomCornerHorizontalCollisions.Length > 2)
            {
                status = PlayerStatus.WALL_SLIDE;

                if (topCornerHorizontalCollisions.Length > 1 && bottomCornerHorizontalCollisions.Length > 1)
                {
                    transform.position = new Vector3(Mathf.Max(transform.position.x - topCornerHorizontalCollisions[1].distance, transform.position.x - bottomCornerHorizontalCollisions[1].distance), transform.position.y, 0);
                }
                else if (topCornerHorizontalCollisions.Length > 1 && bottomCornerHorizontalCollisions.Length < 2)
                {
                    transform.position = new Vector3(transform.position.x - topCornerHorizontalCollisions[1].distance, transform.position.y, 0);
                }
                else
                {
                    transform.position = new Vector3(transform.position.x - bottomCornerHorizontalCollisions[1].distance, transform.position.y, 0);
                }
            }
        }

        if(speed.y > 0)
        {
            leftCornerVerticalCollisions = Physics2D.LinecastAll(upperLeftCorner, upperLeftCorner + new Vector2(0, Time.deltaTime * speed.y));
            rightCornerVerticalCollisions = Physics2D.LinecastAll(upperRightCorner, upperRightCorner + new Vector2(0, Time.deltaTime * speed.y));

            // Collision between the player and a game object above them.
            if(leftCornerVerticalCollisions.Length + rightCornerVerticalCollisions.Length > 2)
            {
                // Cancelling the vertical speed transition if it exists.
                if(verticalSpeedTransition != null)
                {
                    verticalSpeedTransition.Available = true;
                    verticalSpeedTransition = null;
                }
  
                speed.y = -0.3f * speed.y;

                if (leftCornerVerticalCollisions.Length > 1 && rightCornerVerticalCollisions.Length > 1)
                {
                    transform.position = new Vector3(transform.position.x, Mathf.Min(transform.position.y + leftCornerVerticalCollisions[1].distance, transform.position.y + rightCornerVerticalCollisions[1].distance), 0);
                }
                else if (leftCornerVerticalCollisions.Length > 1 && rightCornerVerticalCollisions.Length < 2)
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y + leftCornerVerticalCollisions[1].distance, 0);
                }
                else
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y + rightCornerVerticalCollisions[1].distance, 0);
                }
            }
        }
        else if (speed.y < 0)
        {
            leftCornerVerticalCollisions = Physics2D.LinecastAll(lowerLeftCorner, lowerLeftCorner + new Vector2(0, Time.deltaTime * speed.y));
            rightCornerVerticalCollisions = Physics2D.LinecastAll(lowerRightCorner, lowerRightCorner + new Vector2(0, Time.deltaTime * speed.y));
            
            // Collision between the player and a game object bellow them.
            if (leftCornerVerticalCollisions.Length + rightCornerVerticalCollisions.Length > 2)
            {
                // Cancelling the vertical speed transition if it exists.
                if (verticalSpeedTransition != null)
                {
                    verticalSpeedTransition.Available = true;
                    verticalSpeedTransition = null;
                }

                speed.y = 0;

                if (!Grounded) { status = PlayerStatus.GROUNDED_IDLE; }

                Grounded = true;

                // Resetting the player's number of available jumps.
                numberOfJumpsLeft = maximumNumberOfJumps;

                if (leftCornerVerticalCollisions.Length > 1 && rightCornerVerticalCollisions.Length > 1)
                {
                    transform.position = new Vector3(transform.position.x, Mathf.Max(transform.position.y - leftCornerVerticalCollisions[1].distance, transform.position.y - rightCornerVerticalCollisions[1].distance), 0);
                }
                else if (leftCornerVerticalCollisions.Length > 1 && rightCornerVerticalCollisions.Length < 2)
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y - leftCornerVerticalCollisions[1].distance, 0);
                }
                else
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y - rightCornerVerticalCollisions[1].distance, 0);
                }
            }
        }

        // Updating the palyer's status if necessary
        updateStatus(status);

        Debug.Log(CurrentStatus);

        // Position update.
        transform.position += speed * Time.deltaTime;
    }
}
