using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerScript : MonoBehaviour
{
    private InputAction horizontalMovementInput;
    private InputAction jumpInput;
    private InputAction dashInput;
    private InputAction attackInput;

    private Rigidbody2D playerRB;
    private Animator playerAR;
    private SpriteRenderer playerSR;

    [SerializeField] private int groundLayerInt;
    [SerializeField] private LayerMask groundLayer;
    private PlayerState playerState;
    private enum PlayerState
    {
        Idle = 0,
        Sprint = 1,
        Jump = 2,
        Dash = 3,
        Attack = 4
    }
    void Start()
    {
        horizontalMovementInput = InputSystem.actions.FindAction("Horizontal Movement");
        dashInput = InputSystem.actions.FindAction("Dash");
        jumpInput = InputSystem.actions.FindAction("Jumping");
        attackInput = InputSystem.actions.FindAction("Base Attack");

        playerRB = GetComponent<Rigidbody2D>();
        playerAR = GetComponent<Animator>();
        playerSR = GetComponent<SpriteRenderer>();

        playerState = PlayerState.Idle;
    }
    private void FixedUpdate()
    {
        Sprinting();
        ExcuteJump();
        ExecuteDash();
    }
    void Update()
    {
        CheckForJump();
        CheckForDash();

        SetState();
        StateEffects();
    }
    //--------------------------------------------------| MOVEMENT FUNCTIONS
    private float sprintSpeed = 5f;
    private float horizontalMovement = 0f;
    private Vector2 velocity = new Vector2(0f , 0f);
    private void Sprinting()
    {
        horizontalMovement = horizontalMovementInput.ReadValue<float>();

        if (Mathf.Abs(horizontalMovement) > 0)
            velocity.x = sprintSpeed * horizontalMovement;
        else
            velocity.x = Mathf.Lerp(playerRB.linearVelocityX , 0 , 0.01f);

        velocity.y = playerRB.linearVelocityY;
        playerRB.linearVelocity = velocity;
    }

    private bool dashing = false;
    private void CheckForDash()
    {
        if (!DoingSomething() && dashInput.triggered)
        {
            direction = horizontalMovementInput.ReadValue<float>();
            dashing = true;
            
            currentDashSpeed = dashSpeedPeak * direction;
            sprintSpeedForDashing = sprintSpeed * direction;
        }
    }

    private Vector2 velocityForDashing = new Vector2();
    private const float dashSpeedPeak = 20f;
    private float sprintSpeedForDashing = 5f;
    private float currentDashSpeed;
    private float direction;
    private void ExecuteDash()
    {
        if (dashing)
        {
            if (Mathf.Abs(currentDashSpeed) > 10f)
                currentDashSpeed = Mathf.Lerp(currentDashSpeed , sprintSpeedForDashing , 0.1f);
            else
                currentDashSpeed = Mathf.Lerp(currentDashSpeed, sprintSpeedForDashing , 0.3f);

            velocityForDashing.x = currentDashSpeed;
            velocityForDashing.y = playerRB.linearVelocityY;

            playerRB.linearVelocity = velocityForDashing;

            if (Mathf.Abs(playerRB.linearVelocityX) <= (sprintSpeed + 0.5f))
                dashing = false;
        }
    }

    private bool jumping = false;
    private const float initialJumpVel = 5f;
    private float currentJumpVal;
    private void CheckForJump()
    {
        if (!DoingSomething() && jumpInput.triggered)
        {
            playerAR.SetFloat("Vertical Speed" , initialJumpVel);

            jumping = true;
            jumpVelocityUpdated = false; //Used to account for physics updates and frame update offsets.

            trackedRiseTime = 0f;
            currentJumpVal = initialJumpVel;
        }
    }
    private Vector2 velocityForJumping = new Vector2(0f , 0f);
    private const float allowedAdditionalRiseTime = 0.25f;
    private const float jumpIncr = .35f;
    private float trackedRiseTime;
    private bool jumpVelocityUpdated = false;
    private void ExcuteJump()
    {
        if (jumping)
        {
            if (!jumpVelocityUpdated)
                jumpVelocityUpdated = true;

            if ((trackedRiseTime < allowedAdditionalRiseTime) && (jumpInput.ReadValue<float>() == 1))
                trackedRiseTime += Time.deltaTime;

            else if ((trackedRiseTime < allowedAdditionalRiseTime) && (jumpInput.ReadValue<float>() == 0))
                trackedRiseTime = allowedAdditionalRiseTime + 1f;

            else
                currentJumpVal -= jumpIncr;

            velocityForJumping.x = playerRB.linearVelocityX;
            velocityForJumping.y = currentJumpVal;

            playerRB.linearVelocity = velocityForJumping;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == groundLayerInt)
        {
            if (jumping)
            {
                jumping = false;
                playerAR.SetBool("Jumping" , false);
                playerAR.SetBool("Jump Landing Done" , false);
            }
        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.layer == groundLayerInt)
            Debug.Log(Time.frameCount);
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == groundLayerInt)
            Debug.Log("Left ground");
    }
    private void JumpLandingDone() //For animator use only
    {
        playerAR.SetBool("Jump Landing Done" , true);
    }
    //--------------------------------------------------| MOVEMENT FUNCTIONS
    //--------------------------------------------------| ATTACKING FUNCTIONS
    private bool attacking = false;

    private void CheckForAttack()
    {
        if (!DoingSomething() && attackInput.triggered)
        {
            attacking = true;
        }
    }

    private void ExecuteAttack()
    {

    }
    //--------------------------------------------------| ATTACKING FUNCTIONS
    //--------------------------------------------------|  
    private bool DoingSomething()
    {
        if (jumping)
            return true;

        else if (dashing)
            return true;

        else if (attacking)
            return true;

        else
            return false;
    }
    //--------------------------------------------------|  
    //--------------------------------------------------| STATE SETTING FUNCTIONS
    private void SetState()
    {
        if (jumping)
            playerState = PlayerState.Jump;

        else if (dashing)
            playerState = PlayerState.Dash;

        else if (attacking)
            playerState = PlayerState.Attack;

        else if (Mathf.Abs(playerRB.linearVelocityX) <= 0.01f)
            playerState = PlayerState.Idle;

        else if (Mathf.Abs(playerRB.linearVelocityX) > 0.01f)
            playerState = PlayerState.Sprint;
    }

    private void StateEffects()
    {
        if (playerState == PlayerState.Idle)
            playerAR.SetBool("Sprinting" , false);
        
        else if (playerState == PlayerState.Sprint)
        {
            playerAR.SetBool("Sprinting" , true);
            playerAR.SetBool("Dashing", false);

            playerSR.flipX = (playerRB.linearVelocityX < 0 ? true : false);
        }

        else if (playerState == PlayerState.Dash)
        {
            playerAR.SetBool("Dashing" , true);

            playerSR.flipX = (playerRB.linearVelocityX < 0 ? true : false);
        }

        else if (playerState == PlayerState.Jump)
        {
            playerAR.SetBool("Jumping" , true);
            
            if (jumpVelocityUpdated)
                playerAR.SetFloat("Vertical Speed" , playerRB.linearVelocityY);
            
            playerSR.flipX = (playerRB.linearVelocityX < 0 ? true : false);
        }
    }
    //--------------------------------------------------| STATE SETTING FUNCTIONS
}