using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    // The player instance controlled by the input manager.
    private Player player;

    // Storing the values of previous directional inputs allows to 
    // compare these values with those of current inputs.
    // This is useful to make the player run with the joystick only by 
    // pressing it fast to the right or left (if the absolute value of the difference 
    // between the previous and current values is high enough), similarly to the 
    // way it is done in the Super Smash Bros series for instance.
    private float lastHorizontalInput;
    private float lastVerticalInput;
    private float currentHorizontalInput;
    private float currentVerticalInput;

    [SerializeField]
    private float runningThreshold = 0.7f;

    [SerializeField]
    private float crouchingThreshold = 0.7f;

    private float rushValue = 0.6f;

    // Start is called before the first frame update
    void Start()
    {
        if(runningThreshold < 0 || runningThreshold > 1) { runningThreshold = 0.7f; }
        if(crouchingThreshold < 0 || crouchingThreshold > 1) { crouchingThreshold = 0.7f; }

        GameObject playerGameObject = GameObject.Find("Player");
        if(!playerGameObject) { Debug.LogError("No game object nammed 'Player' was found."); Application.Quit(); }

        player = playerGameObject.GetComponent<Player>();
        if(!player) { Debug.LogError("The player game object has no component of class Player."); Application.Quit(); }

        lastHorizontalInput = 0;
        lastVerticalInput = 0;
        currentHorizontalInput = 0;
        currentVerticalInput = 0;
    }

    // Update is called once per frame
    void Update()
    {
        bool joystickInput = false;

        PlayerStatus status;

        if(player.Grounded)
        {
            status = PlayerStatus.GROUNDED_IDLE;
        }
        else { status = PlayerStatus.MIDAIR_IDLE; }

        // Jump
        if(Input.GetKey(KeyCode.Joystick1Button5) || Input.GetKey(KeyCode.Space))
        {
            player.Grounded = false;
            status = PlayerStatus.MIDAIR_IDLE;
            player.updateStatus(status);
        }

        // Directional inputs
        lastHorizontalInput = currentHorizontalInput;
        lastVerticalInput = currentVerticalInput;

        if(Input.GetAxis("JoystickHorizontal") + Input.GetAxis("JoystickVertical") != 0)
        {
            currentHorizontalInput = Input.GetAxis("JoystickHorizontal");
            currentVerticalInput = Input.GetAxis("JoystickVertical");

            joystickInput = true;
        }
        else if(Input.GetAxis("DPadHorizontal") + Input.GetAxis("DPadVertical") != 0)
        {
            currentHorizontalInput = Input.GetAxis("DPadHorizontal");
            currentVerticalInput = Input.GetAxis("DPadVertical");
        }
        else
        {
            currentHorizontalInput = Input.GetAxis("KeyboardHorizontal");
            currentVerticalInput = Input.GetAxis("KeyboardVertical");
        }

        if (currentHorizontalInput != 0)
        {
            player.Orientation = (int)(currentHorizontalInput / Mathf.Abs(currentHorizontalInput));
        }
        Debug.Log(player.Grounded);
        if (player.Grounded) 
        {
            if(currentVerticalInput <= -crouchingThreshold)
            {
                status = PlayerStatus.GROUND_SLIDE;

            }
            else if (currentHorizontalInput != 0)
            {

                if (
                    (
                    joystickInput
                    &&
                    Mathf.Abs(currentHorizontalInput) >= runningThreshold
                    )
                    &&
                    (
                    player.CurrentStatus == PlayerStatus.RUN
                    ||
                    (
                    lastHorizontalInput * currentHorizontalInput >= 0
                    &&
                    Mathf.Abs(currentHorizontalInput) >= Mathf.Abs(lastHorizontalInput)
                    &&
                    Mathf.Abs(currentHorizontalInput - lastHorizontalInput) >= rushValue
                    )
                    ||
                    (
                    lastHorizontalInput * currentHorizontalInput < 0
                    &&
                    Mathf.Abs(currentHorizontalInput - lastHorizontalInput) >= 2 * rushValue)
                    )
                    )
                {
                    status = PlayerStatus.RUN;

                }
                else if (!joystickInput && (Input.GetKey(KeyCode.Joystick1Button2) || Input.GetKey(KeyCode.X)))
                {
                    status = PlayerStatus.RUN;
                }
                else
                {
                    status = PlayerStatus.WALK;
                }
            }
        }
        // status == PlayerStatus.MIDAIR_IDLE
        else
        {
            if(currentHorizontalInput != 0)
            {
                if(player.CurrentStatus == PlayerStatus.WALL_SLIDE)
                {

                }

                if(Input.GetKey(KeyCode.Joystick1Button4) || Input.GetKey(KeyCode.D))
                {
                    status = PlayerStatus.DASH;
                }

                status = PlayerStatus.AIR_CONTROL;
            }
        }

        player.updateStatus(status);
    }
}
