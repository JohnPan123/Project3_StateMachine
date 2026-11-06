using UnityEngine;
using UnityEngine.UIElements;

public class MushroomScript : MonsterScript
{
    private Rigidbody2D mushroomRB;
    private BaseMoveStates moveState;
    private MushroomStates mushroomState;
    private enum BaseMoveStates
    {
        Idle = 0,
        MoveLeft = 1,
        IdleTwo = 2,
        MoveRight = 3
    }
    private enum MushroomStates
    {
        Idle = 0,
        Move = 1,
        Attack = 2,
        AttackMove = 3,
        Death = 4
    }
    void Start()
    {
        player = GameObject.Find(PlayerScript.playerName);

        monsterAR = GetComponent<Animator>();
        monsterSR = GetComponent<SpriteRenderer>();
        mushroomRB = GetComponent<Rigidbody2D>();

        mushroomState = MushroomStates.Idle;
        moveState = BaseMoveStates.Idle;
    }

    private void FixedUpdate()
    {
        MushroomMove();
    }
    void Update()
    {
        MonsterDetectAttack();

        SetState();
        StateEffects();
    }
    //--------------------------------------------------| MOVEMENT FUNCTIONS
    private void MushroomMove()
    {
        if (attackMove)
            MonsterAttackMove();

        else
            MonsterBaseMove();
    }


                                private Vector3 moveVector = new Vector3(monsterSpeed , 0f , 0f);
                                private const float monsterSpeed = 3.5f;
                                [SerializeField] private float timeToChangeMove = 1f;
                                private float timeTillMoveChange = 0f;
    protected override void MonsterBaseMove()
    {
        timeTillMoveChange += Time.deltaTime;

        if (timeTillMoveChange >= timeToChangeMove)
        {
            if (((int)moveState + 1) == 4)
                moveState = BaseMoveStates.Idle;

            else
                moveState++;

            timeTillMoveChange = 0f;
        }

        switch (moveState)
        {
            case BaseMoveStates.Idle:
                mushroomRB.linearVelocity = Vector2.zero;
                break;
            case BaseMoveStates.MoveLeft:
                mushroomRB.linearVelocity = -moveVector;
                break;
            case BaseMoveStates.IdleTwo:
                mushroomRB.linearVelocity = Vector2.zero;

                break;
            case BaseMoveStates.MoveRight:
                mushroomRB.linearVelocity = moveVector;
                break;
        }
    }


                                private bool attacking = false;
                                private Vector3 moveVectorAttack = new Vector3(0 , 0 , 0);
                                private const float demonAttackSpeed = 4.5f;
    protected override void MonsterAttackMove()
    {
        if (transform.position.x <= player.transform.position.x)
        {
            moveVectorAttack.x = demonAttackSpeed;
            moveVectorAttack.y = mushroomRB.linearVelocityY;

            mushroomRB.linearVelocity = (Vector2)moveVectorAttack;
            moveState = BaseMoveStates.MoveRight;
        }

        else if (transform.position.x > player.transform.position.x)
        {
            moveVectorAttack.x = -demonAttackSpeed;
            moveVectorAttack.y = mushroomRB.linearVelocityY;

            mushroomRB.linearVelocity = (Vector2)moveVectorAttack;
            moveState = BaseMoveStates.MoveLeft;
        }
    }

    private void ExecuteAttack()
    {

    }
    //--------------------------------------------------| MOVEMENT FUNCTIONS
    //--------------------------------------------------| ATTACK FUNCTIONS
                                private const float verticalViewRange = 1f;
                                private const float horizontalViewRange = 3f;
                                private Vector2 rayDirection = new Vector2(0, 0);
                                private bool attackMove = false;
    protected override void MonsterDetectAttack()
    {
        for (float verticalComponenet = 0f; verticalComponenet <= verticalViewRange; verticalComponenet += 0.2f)
        {
            rayDirection.x = (monsterSR.flipX ? 1 : -1) * horizontalViewRange;
            rayDirection.y = verticalComponenet;

            if (Physics2D.Raycast(transform.position, rayDirection, rayDirection.magnitude, playerLayer))
            {
                attackMove = true;
                break;
            }
        }
    }
    //--------------------------------------------------| ATTACK FUNCTIONS
    //--------------------------------------------------| HEALTH FUNCTIONS
    private bool dead = false;
    public override void LoseHealth(int healthLost)
    {
        throw new System.NotImplementedException();
    }

    protected override void Death()
    {
        throw new System.NotImplementedException();
    }
    //--------------------------------------------------| HEALTH FUNCTIONS
    //--------------------------------------------------| STATE SETTING FUNCTIONS
    protected override void SetState()
    {
        DirectionSetting();

        if (dead)
            mushroomState = MushroomStates.Death;
        else if (attackMove)
            mushroomState = MushroomStates.AttackMove;
    }

    protected override void StateEffects()
    {
        if (mushroomState == MushroomStates.Death)
            monsterAR.SetBool("Dead", true);

        else if (mushroomState == MushroomStates.AttackMove)
            monsterAR.SetBool("AttackMove", true);

        else if (moveState == BaseMoveStates.Idle || moveState == BaseMoveStates.IdleTwo)
        {
            monsterAR.SetBool("Idle", true);
            monsterAR.SetBool("Moving", false);
        }

        else if (moveState == BaseMoveStates.MoveLeft || moveState == BaseMoveStates.MoveRight)
        {
            monsterAR.SetBool("Moving", true);
            monsterAR.SetBool("Idle", false);
        }
    }

    private Vector2 attackHitBoxDirectioning = new Vector2(0, 0);
    private void DirectionSetting()
    {
        if (Mathf.Abs(mushroomRB.linearVelocityX) <= 0.01f)
            monsterSR.flipX = monsterSR.flipX;
        
        else
            monsterSR.flipX = (mushroomRB.linearVelocityX < 0 ? false : true);

    }
    //--------------------------------------------------| STATE SETTING FUNCTIONS
}
