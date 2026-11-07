using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Drawing.Inspector.PropertyDrawers;

public class PlayerScript : MonoBehaviour
{
    private InputAction horizontalMovementInput;
    private InputAction jumpInput;
    private InputAction dashInput;
    private InputAction attackInput;

    private Rigidbody2D playerRB;
    private Animator playerAR;
    private SpriteRenderer playerSR;

    private float playerHealth = 100f;
    private const float playerZeroSpeed = 0.1f;

    [SerializeField] private List<BoxCollider2D> attackHitBoxes;
    private Dictionary<string, int> landingLayersDict = new Dictionary<string , int>();
    [SerializeField] private LayerMask landingLayersMask;
    
    private PlayerState playerState;
    public static string playerName;
    private enum PlayerState
    {
        Idle = 0,
        Sprint = 1,
        Jump = 2,
        Dash = 3,
        Attack = 4,
        ReceivedHit = 5
    }

    void Start()
    {
        horizontalMovementInput = InputSystem.actions.FindAction("Horizontal Movement");
        dashInput = InputSystem.actions.FindAction("Dash");
        jumpInput = InputSystem.actions.FindAction("Jumping");
        attackInput = InputSystem.actions.FindAction("Base Attack");

        landingLayersDict.Add("Ground Layer" , LayerMask.NameToLayer("Ground"));
        landingLayersDict.Add("Platform Layer" , LayerMask.NameToLayer("Platform"));
        landingLayersDict.Add("Island Layer", LayerMask.NameToLayer("Island"));
        landingLayersDict.Add("Island Under Layer" , LayerMask.NameToLayer("IslandUnderPart"));

        playerRB = GetComponent<Rigidbody2D>();
        playerAR = GetComponent<Animator>();
        playerSR = GetComponent<SpriteRenderer>();

        playerName = gameObject.name;
        playerState = PlayerState.Idle;
    }

    private void FixedUpdate()
    {
        ExecuteGravity();

        if (solidHitLanded)
            StaggerMovement();
        else
            HorizontalMovement();

        ExecuteJump();
        ExecuteDash();
        ExecuteAttack();
    }
    void Update()
    {
        //DebugFunction();

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
                                    private bool stillJump = false;
                                    private bool jumpVelocityUpdated = false; //Used to account for physics updates and frame update offsets.
                                    private const float initialJumpVel = 5f;
                                    private float currentJumpVal;
    private void CheckForJump()
    {
        if (!DoingSomething() && jumpInput.triggered)
        {
            playerAR.SetFloat("Vertical Speed" , initialJumpVel);
            
            jumping = true;
            jumpVelocityUpdated = false;
            
            if (Mathf.Abs(playerRB.linearVelocityX) <= playerZeroSpeed)
                stillJump = true;

            trackedRiseTime = 0f;
            currentJumpVal = initialJumpVel;
        }
    }

                                    private Vector2 velocityForJumping = new Vector2(0f , 0f);
                                    private const float allowedAdditionalRiseTime = 0.25f;
                                    private const float jumpIncr = .35f;
                                    private float trackedRiseTime;
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

            if (stillJump)
                velocityForJumping.x = 0f;
            else
                velocityForJumping.x = playerRB.linearVelocityX;

            velocityForJumping.y = currentJumpVal;

            playerRB.linearVelocity = velocityForJumping;
        }
    }


                                    private const float staggerMovementSpeed = 3f;
                                    private Vector2 staggerMovement = new Vector2(0 , 0);
    private void StaggerMovement()
    {
        staggerMovement.y = playerRB.linearVelocityY;
        playerRB.linearVelocity = staggerMovement;
    }

                                    private bool solidHitLanded = false;
                                    private bool sameDirection = false;
                                    private bool otherDirection = false;
                                    private GameObject onCollisionEnterGameObject;
    private void OnCollisionEnter2D(Collision2D collision) //Collision function here
    {
        onCollisionEnterGameObject = collision.gameObject;

        if (onCollisionEnterGameObject.layer == landingLayersDict["Ground Layer"] || onCollisionEnterGameObject.layer == landingLayersDict["Platform Layer"] ||
            onCollisionEnterGameObject.layer == landingLayersDict["Island Layer"])
        {
            if (jumping && (currentJumpVal != 5f))
            //If I did not have the second check (currentJumpVal != 5f),
            //sometimes the game would have a collision the same frame jump began
            //and abruptly cut it off. This second check fixed it.
            {
                jumping = stillJump = false;
                playerAR.SetBool("Jumping", false);
                playerAR.SetBool("Jump Landing Done", false);
            }
        }
        
        else if (onCollisionEnterGameObject.layer == landingLayersDict["Island Under Layer"])
            currentJumpVal = 0f;
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        onCollisionEnterGameObject = collision.gameObject;

        if (onCollisionEnterGameObject.CompareTag("MushroomHitBox"))
        {
            solidHitLanded = animationDamageTrigger = true;
            attacking = jumping = dashing = false;

            if (onCollisionEnterGameObject.transform.position.x > transform.position.x)
            {
                staggerMovement.x = -staggerMovementSpeed;

                if (playerSR.flipX)
                    sameDirection = true;
                else
                    otherDirection = true;
            }
            else
            {
                staggerMovement.x = staggerMovementSpeed;

                if (playerSR.flipX)
                    otherDirection = true;
                else
                    sameDirection = true;
            }

        }
    }
    private bool animationDamageTrigger = false;
    private void FinishedTakingDamage() //For animator
    {
        solidHitLanded = otherDirection = sameDirection = false;
    }

    private void JumpLandingDone() //For animator use only
    {
        playerAR.SetTrigger("Jump Landing Done");
    }


                                    [SerializeField] private float gravityMulitplier = 7f;
                                    private const float drawDistance = 0.79f;
                                    private Vector3 playerWidthOffset = new Vector3(0.23f , 0f , 0f);
                                    private Vector2 velocityDueToGravity = new Vector2(0 , -9.81f  * (1f / 50f));
                                    private Vector2 onGroundVelocityDueToGravity = new Vector2(0 , 0);
    private void ExecuteGravity()
    {
        //Debug.DrawRay(transform.position + playerWidthOffset, Vector2.down * drawDistance , Color.red);
        if ((Physics2D.Raycast(transform.position + playerWidthOffset , Vector2.down, drawDistance , landingLayersMask).collider != null) || 
            (Physics2D.Raycast(transform.position - playerWidthOffset , Vector2.down, drawDistance , landingLayersMask).collider != null))
            playerRB.linearVelocity += onGroundVelocityDueToGravity;
        else
            playerRB.linearVelocity += (velocityDueToGravity * gravityMulitplier);
    }
    //--------------------------------------------------| MOVEMENT FUNCTIONS
    //--------------------------------------------------| ATTACKING FUNCTIONS
                                    private bool attacking = false;
                                    private bool nextAttackConfirmed = false;
    private void CheckForAttack()
    {
        if (!DoingSomething() && attackInput.triggered)
            attacking = nextAttackConfirmed = true;

        else if (attacking && attackInput.triggered)
            nextAttackConfirmed = true;
    }

    private void CheckForNextAttack() //For use by animator
    {
        if (!nextAttackConfirmed)
            attacking = false;

        nextAttackConfirmed = false;
    }

    private void ExecuteAttack()
    {
        if (attacking)
            playerRB.linearVelocity = Vector2.zero;
    }
    //--------------------------------------------------| ATTACKING FUNCTIONS
    //--------------------------------------------------| ATTACKING HITBOX FUNCTIONS
                                    private const int locationOfHitBoxNumber = 12; //The names for the base attacks are all Base Attack n.
                                    private int hitBox = 0;
    private void BaseAttackHitBoxActivation()
    {
        hitBox = int.Parse(playerAR.GetCurrentAnimatorClipInfo(0)[0].clip.name.Substring(locationOfHitBoxNumber));
        //The minus one is needed for the array indexing which starts at zero. The first attack is base attack 1 and that
        //refers to the second hit box in array indexing.
        attackHitBoxes[hitBox - 1].enabled = true;
    }
    private void BaseAttackHitBoxDeactivation()
    {
        attackHitBoxes[hitBox - 1].enabled = false;
    }
    //--------------------------------------------------| ATTACKING HITBOX FUNCTIONS
    //--------------------------------------------------|  MISCELLANEOUS FUNCTIONS
    private bool DoingSomething()
    {
        if (solidHitLanded)
            return true;

        else if (jumping)
            return true;

        else if (dashing)
            return true;

        else if (attacking)
            return true;

        else
            return false;
    }

    private void DebugFunction()
    {
        Debug.Log($"Solid Hit: {solidHitLanded} || Jumping: {jumping} || Dashing: {dashing} || Attacking: {attacking}");
    }
    //--------------------------------------------------|  MISCELLANEOUS FUNCTIONS
    //--------------------------------------------------|  PLAYER HEALTH FUNCTIONS
    public void HealthLost(float healthLost)
    {
        playerHealth -= healthLost;
    }
    public float GetPlayerHealth => playerHealth;
    //--------------------------------------------------| PLAYER HEALTH FUNCTIONS
    //--------------------------------------------------| STATE SETTING FUNCTIONS
    private void SetState()
    {
        if (solidHitLanded)
            playerState = PlayerState.ReceivedHit;

        else if (jumping)
            playerState = PlayerState.Jump;

        else if (dashing)
            playerState = PlayerState.Dash;

        else if (attacking)
            playerState = PlayerState.Attack;

        else if (Mathf.Abs(playerRB.linearVelocityX) > playerZeroSpeed)
            playerState = PlayerState.Sprint;

        else if (Mathf.Abs(playerRB.linearVelocityX) <= playerZeroSpeed)
            playerState = PlayerState.Idle;
    }

    private void StateEffects()
    {
        if (jumpVelocityUpdated)
            playerAR.SetFloat("Vertical Speed", playerRB.linearVelocityY);

        if (playerState == PlayerState.Idle)
        {
            playerAR.SetTrigger("Idle");
        }

        else if (playerState == PlayerState.Sprint)
        {
            playerAR.SetTrigger("Sprinting");
        }

        else if (playerState == PlayerState.Dash)
        {
            playerAR.SetTrigger("Dashing");
        }

        else if (playerState == PlayerState.Jump)
        {
            playerAR.SetBool("Jumping" , true);
        }

        else if (playerState == PlayerState.Attack)
        {
            if (nextAttackConfirmed)
                playerAR.SetTrigger("Attacking");
        }

        else if (playerState == PlayerState.ReceivedHit)
        {
            if (animationDamageTrigger)
            {
                playerAR.SetTrigger("Received Hit");
                playerAR.SetBool("Jumping", false);

                if (sameDirection)
                    playerAR.SetBool("Hit Direction", true);

                else if (otherDirection)
                    playerAR.SetBool("Hit Direction", false);

                animationDamageTrigger = false;
            }
            
        }

        DirectionSetting();
    }

    private Vector2 attackHitBoxDirectioning = new Vector2(0 , 0);
    private void DirectionSetting()
    {
        if ((Mathf.Abs(playerRB.linearVelocityX) <= playerZeroSpeed) || 
            (horizontalMovementInput.ReadValue<float>() == 0) || solidHitLanded)
            playerSR.flipX = playerSR.flipX;

        else
        {
            playerSR.flipX = (playerRB.linearVelocityX < 0 ? true : false);

            if (playerSR.flipX)
            {
                foreach (BoxCollider2D box in attackHitBoxes)
                {
                    attackHitBoxDirectioning.x = -1 * Mathf.Abs(box.offset.x);
                    attackHitBoxDirectioning.y = -1 * Mathf.Abs(box.offset.y);

                    box.offset = attackHitBoxDirectioning;
                }
            }

            else
            {
                foreach (BoxCollider2D box in attackHitBoxes)
                {
                    attackHitBoxDirectioning.x = Mathf.Abs(box.offset.x);
                    attackHitBoxDirectioning.y = Mathf.Abs(box.offset.y);

                    box.offset = attackHitBoxDirectioning;
                }
            }
        }

    }
    //--------------------------------------------------| STATE SETTING FUNCTIONS
}