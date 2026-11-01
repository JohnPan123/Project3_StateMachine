using System;
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
        ExecuteGravity();

        HorizontalMovement();
        ExecuteJump();
        ExecuteDash();
        ExecuteAttack();
    }
    void Update()
    {
        CheckForJump();
        CheckForDash();
        CheckForAttack();

        SetState();
        StateEffects();
    }
    //--------------------------------------------------| MOVEMENT FUNCTIONS
    private float sprintSpeed = 5f;
    private float horizontalMovement = 0f;
    private Vector2 velocity = new Vector2(0f , 0f);
    private void HorizontalMovement()
    {
        horizontalMovement = horizontalMovementInput.ReadValue<float>();

        if (Mathf.Abs(horizontalMovement) > 0)
            velocity.x = sprintSpeed * horizontalMovement;

        else
            velocity.x = 0f;

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

            if (Mathf.Abs(playerRB.linearVelocityX) <= (sprintSpeed + 0.1f))
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
    private void ExecuteJump()
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

    private bool onGround = false;
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

    private void JumpLandingDone() //For animator use only
    {
        playerAR.SetBool("Jump Landing Done" , true);
    }

    [SerializeField] private float gravityMulitplier = 7f;
    private const float drawDistance = 0.79f;
    private Vector3 playerWidthOffset = new Vector3(0.23f , 0f , 0f);
    private Vector2 velocityDueToGravity = new Vector2(0 , -9.81f  * (1f / 50f));
    private Vector2 onGroundVelocityDueToGravity = new Vector2(0 , 0);
    private void ExecuteGravity()
    {
        //Debug.DrawRay(transform.position + playerWidthOffset, Vector2.down * drawDistance , Color.red);
        if ((Physics2D.Raycast(transform.position + playerWidthOffset , Vector2.down, drawDistance , groundLayer).collider != null) || 
            (Physics2D.Raycast(transform.position - playerWidthOffset , Vector2.down, drawDistance , groundLayer).collider != null))
                playerRB.linearVelocity += onGroundVelocityDueToGravity;
        else
            playerRB.linearVelocity += (velocityDueToGravity * gravityMulitplier);

    }
    //--------------------------------------------------| MOVEMENT FUNCTIONS
    //--------------------------------------------------| ATTACKING FUNCTIONS
    private bool attacking = false;
    private bool nextAttackConfirmed = false;
    private bool attackTwo = false;
    private bool attackThree = false;
    private void CheckForAttack()
    {
        if (!DoingSomething() && attackInput.triggered)
            attacking = true;

        else if (attacking && attackInput.triggered)
            nextAttackConfirmed = true;
    }

    private void CheckForNextAttack() //For use by animator
    {
        if (nextAttackConfirmed)
        {
            if (!attackTwo)
                attackTwo = true;

            else if (!attackThree)
                attackThree = true;

            else
                attacking = attackTwo = attackThree = false;
        }

        else
            attacking = attackTwo = attackThree = false;

        nextAttackConfirmed = false;
    }

    private Vector2 attackingVelocity = new Vector2(0, 0);
    private void ExecuteAttack()
    {
        if (attacking)
            playerRB.linearVelocity = attackingVelocity;
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

        else if (Mathf.Abs(playerRB.linearVelocityX) > 0.01f)
            playerState = PlayerState.Sprint;

        else if (Mathf.Abs(playerRB.linearVelocityX) <= 0.01f)
            playerState = PlayerState.Idle;
    }

    private void StateEffects()
    {
        if (playerState == PlayerState.Idle)
        {
            playerAR.SetBool("Sprinting", false);
            playerAR.SetBool("Dashing", false);
            playerAR.SetBool("Attacking", false);
        }

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

        else if (playerState == PlayerState.Attack)
        {
            playerAR.SetBool("Attacking" , true);
        }
    }
    //--------------------------------------------------| STATE SETTING FUNCTIONS
}