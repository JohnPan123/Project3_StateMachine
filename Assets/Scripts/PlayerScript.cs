using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerScript : MonoBehaviour
{
    private InputAction horizontalMovementInput;
    private InputAction jumpInput;

    private Rigidbody2D playerRB;
    private Animator playerAR;
    private SpriteRenderer playerSR;

    [SerializeField] private LayerMask groundLayer;
    private PlayerState playerState;
    private enum PlayerState
    {
        Idle = 0,
        Sprint = 1,
        Jump = 2,
        Falling = 3
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        horizontalMovementInput = InputSystem.actions.FindAction("Horizontal Movement");
        jumpInput = InputSystem.actions.FindAction("Jumping");

        playerRB = GetComponent<Rigidbody2D>();
        playerAR = GetComponent<Animator>();
        playerSR = GetComponent<SpriteRenderer>();

        playerState = PlayerState.Idle;
    }
    private void FixedUpdate()
    {
        Sprinting();
        DoJump();
    }
    // Update is called once per frame
    void Update()
    {
        CheckForJump();

        SetState();
        StateEffects();
    }

    private float sprintSpeed = 5f;
    private float horizontalMovement = 0f;
    private Vector2 velocity = new Vector2(0f , 0f);
    private void Sprinting()
    {
        horizontalMovement = horizontalMovementInput.ReadValue<float>();

        if (Mathf.Abs(horizontalMovement) > 0)
            velocity.x = sprintSpeed * horizontalMovement;
        else
            velocity.x = 0;

        velocity.y = playerRB.linearVelocityY;
        playerRB.linearVelocity = velocity;
    }

    private bool jumping = false;
    private const float initialJumpVel = 5f;
    private float currentJumpVal;
    private const float jumpIncr = .27f;
    private void CheckForJump()
    {
        if (!jumping && jumpInput.triggered)
        {
            jumping = true;
            currentJumpVal = initialJumpVel;
        }
    }
    private Vector2 jumpVelocity = new Vector2(0f , 0f);
    private void DoJump()
    {
        if (jumping)
        {
            currentJumpVal -= jumpIncr;

            jumpVelocity.x = playerRB.linearVelocityX;
            jumpVelocity.y = currentJumpVal;

            playerRB.linearVelocity = jumpVelocity;
        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.layer == groundLayer)
        {
            if (jumping)
            {
                jumping = false;
                playerAR.SetBool("Jumping" , false);
                Debug.Log(true);
            }
        }
    }
    private void SetState()
    {
        if (jumping)
            playerState = PlayerState.Jump;

        else if (playerRB.linearVelocityX == 0f)
            playerState = PlayerState.Idle;

        else if (Mathf.Abs(playerRB.linearVelocityX) > 0f)
            playerState = PlayerState.Sprint;
    }

    private void StateEffects()
    {
        if (playerState == PlayerState.Idle)
            playerAR.SetBool("Sprinting" , false);
        
        else if (playerState == PlayerState.Sprint)
        {
            playerAR.SetBool("Sprinting" , true);
            playerSR.flipX = (playerRB.linearVelocityX < 0 ? true : false);
        }

        else if (playerState == PlayerState.Jump)
        {
            playerAR.SetBool("Jumping" , true);
            playerSR.flipX = (playerRB.linearVelocityX < 0 ? true : false);
        }
    }
}