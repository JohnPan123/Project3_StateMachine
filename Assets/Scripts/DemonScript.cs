using UnityEngine;

public class DemonScript : MonoBehaviour
{
    [SerializeField] private LayerMask playerLayer;

    [SerializeField] private BoxCollider2D attackBreathCollider;

    private GameObject player;

    private SpriteRenderer demonSR;
    private Animator demonAR;

    private DemonStates demonState;
    private enum MoveStates
    {
        Idle = 0,
        MoveLeft = 1,
        IdleTwo = 2,
        MoveRight = 3
    }
    private enum DemonStates
    {
        Idle = 0,
        Move = 1,
        SpecialAttack = 2,
        BreathAttack = 3
    }
    void Start()
    {
        player = GameObject.Find(PlayerScript.playerName);

        demonAR = GetComponent<Animator>();
        demonSR = GetComponent<SpriteRenderer>();

        attackBreathCollider.enabled = false;
        demonState = DemonStates.Idle;
    }

    void Update()
    {
        DemonMove();
        
        if (!attacking)
            DemonDetectAttack();

        SetState();
        StateEffects();
    }

    //--------------------------------------------------| MOVEMENT FUNCTIONS
    private const float demonSpeed = 3.5f;
    private const float demonAttackSpeed = 4.5f;

    private Vector3 moveVector = new Vector3(demonSpeed , 0f , 0f);
    private Vector3 leftAttackOffset = new Vector3(-0.5f, 0.5f, 0);
    private Vector3 rightAttackOffset = new Vector3(0.5f , 0.5f , 0);

    private float timeToChangeMove = 1f;
    private float timeTillMoveChange = 0f;

    private MoveStates moveState = MoveStates.Idle;
    private void DemonMove()
    {
        if (!attacking)
        {
            if (attackBreathCollider.enabled)
                attackBreathCollider.enabled = false;

            timeTillMoveChange += Time.deltaTime;

            if (timeTillMoveChange >= timeToChangeMove)
            {
                if (((int)moveState + 1) == 4)
                    moveState = MoveStates.Idle;
                else
                    moveState++;

                timeTillMoveChange = 0f;
            }

            switch (moveState)
            {
                case MoveStates.Idle:
                    break;
                case MoveStates.MoveLeft:
                    transform.Translate(-moveVector * Time.deltaTime);
                    break;
                case MoveStates.IdleTwo:
                    break;
                case MoveStates.MoveRight:
                    transform.Translate(moveVector * Time.deltaTime);
                    break;
            }
        }

        else if (attacking)
        {
            if (!attackBreathCollider.enabled)
                attackBreathCollider.enabled = true;


            if (transform.position.x < player.transform.position.x)
            {
                moveVector = (player.transform.position - transform.position + leftAttackOffset).normalized;
                moveState = MoveStates.MoveRight;
            }
            else
            {
                moveVector = (player.transform.position - transform.position + rightAttackOffset).normalized;
                moveState = MoveStates.MoveLeft;
            }

            transform.Translate(moveVector * demonAttackSpeed * Time.deltaTime);
        }

    }
    //--------------------------------------------------| MOVEMENT FUNCTIONS
    //--------------------------------------------------| ATTACKING FUNCTIONS

    private const float verticalViewRange = 1f;
    private const float horizontalViewRange = 3f;
    private Vector2 rayDirection = new Vector2(0 , 0);
    private bool attacking = false;
    private void DemonDetectAttack()
    {
        for (float verticalComponenet = 0f ; verticalComponenet <= verticalViewRange ; verticalComponenet += 0.2f)
        {
            rayDirection.x = (demonSR.flipX ? 1 : -1) * horizontalViewRange;
            rayDirection.y = -verticalComponenet;

            if (Physics2D.Raycast(transform.position, rayDirection, rayDirection.magnitude, playerLayer))
            {
                attacking = true;
                break;
            }
        }
    }
    //--------------------------------------------------| ATTACKING FUNCTIONS
    //--------------------------------------------------| STATE SETTING FUNCTIONS
    private void SetState()
    {
        if (attacking)
            demonState = DemonStates.BreathAttack;

        if (moveState == MoveStates.MoveLeft)
            demonSR.flipX = false;
        else if (moveState == MoveStates.MoveRight)
            demonSR.flipX = true;
    }

    private void StateEffects()
    {
        if (demonState == DemonStates.BreathAttack)
            demonAR.SetBool("Attacking" , true);
    }
    //--------------------------------------------------| STATE SETTING FUNCTIONS
}
