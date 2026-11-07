using UnityEngine;

public class MushroomScript : MonsterScript
{
    private BoxCollider2D mushroomHB;
    private Rigidbody2D mushroomRB;
    private MushroomStates mushroomState;
    private enum MushroomStates
    {
        Idle = 0,
        Move = 1,
        Attack = 2,
        Death = 3
    }
    void Start()
    {
        player = GameObject.Find(PlayerScript.playerName);

        monsterAR = GetComponent<Animator>();
        monsterSR = GetComponent<SpriteRenderer>();
        mushroomRB = GetComponent<Rigidbody2D>();
        mushroomHB = GetComponent<BoxCollider2D>();


        mushroomState = MushroomStates.Idle;
        monsterHealth = 3;
    }

    private void FixedUpdate()
    {
        if (!idleAfterAttackBool)
            MushroomMove();
    }
    void Update()
    {
        if (!idleAfterAttackBool) { MonsterDetectAttack(); }
        else { AfterAttackIdle(); }

        SetState();
        StateEffects();
    }
    //--------------------------------------------------| MOVEMENT FUNCTIONS
    private void MushroomMove()
    {
        if (idleAfterAttackBool || attacking) 
            ;
        else if (attackMove)
            MonsterAttackMove();
        else
            MonsterBaseMove();
    }


                                private Vector3 moveVector = new Vector3(monsterSpeed , 0f , 0f);
                                private const float monsterSpeed = 3.5f;
                                private const float physicsTickTime = (1f / 50f);
                                [SerializeField] private float distanceToChangeMove;
                                [SerializeField] private float idleTime = 2f;
                                private float xDistanceTravelledTracker = 0f;
                                private float idleTimeTracker = 0f;
                                private int movePhase = 0;
    protected override void MonsterBaseMove()
    {
        xDistanceTravelledTracker += Mathf.Abs(mushroomRB.linearVelocityX) * physicsTickTime;

        if ((movePhase % 2) == 0)
            idleTimeTracker += Time.deltaTime;

        if ((xDistanceTravelledTracker >= distanceToChangeMove) || idleTimeTracker >= idleTime)
        {
            if ((movePhase + 1) == 4)
                movePhase = 0;
            else
                movePhase++;

            idleTimeTracker = 0f;
            xDistanceTravelledTracker = 0f;
        }

        switch (movePhase)
        {
            case 0: //Idle
                moveVector.x = 0f;
                moveVector.y = mushroomRB.linearVelocityY;

                mushroomRB.linearVelocity = Vector2.zero;
                break;
            case 1: //Move left
                moveVector.x = -monsterSpeed;
                moveVector.y = mushroomRB.linearVelocityY;

                mushroomRB.linearVelocity = moveVector;
                break;
            case 2: //Idle
                moveVector.x = 0f;
                moveVector.y = mushroomRB.linearVelocityY;

                mushroomRB.linearVelocity = Vector2.zero;
                break;
            case 3: //Move right
                moveVector.x = monsterSpeed;
                moveVector.y = mushroomRB.linearVelocityY;

                mushroomRB.linearVelocity = moveVector;
                break;
        }
    }


                                private bool attacking = false;
                                private Vector2 moveVectorAttack = new Vector3(0 , 0);
                                private const float mushroomAttackSpeed = 4.5f;
    protected override void MonsterAttackMove()
    {
        if (transform.position.x < player.transform.position.x)
        {
            moveVectorAttack.x = mushroomAttackSpeed;
            moveVectorAttack.y = mushroomRB.linearVelocityY;

            mushroomRB.linearVelocity = moveVectorAttack;
        }

        else if (transform.position.x > player.transform.position.x)
        {
            moveVectorAttack.x = -mushroomAttackSpeed;
            moveVectorAttack.y = mushroomRB.linearVelocityY;

            mushroomRB.linearVelocity = moveVectorAttack;
        }
    }
    //--------------------------------------------------| MOVEMENT FUNCTIONS
    //--------------------------------------------------| ATTACK FUNCTIONS
                                private const float verticalViewRange = 0.7f;
                                private const float horizontalViewRange = 3f;
                                private Vector3 mushroomHeightOffset = new Vector3(0 , 0.5f , 0);
                                private Vector2 rayDirection = new Vector2(0 , 0);
                                private bool attackMove = false;
    protected override void MonsterDetectAttack()
    {
        for (float verticalComponenet = -0.35f ; verticalComponenet <= verticalViewRange ; verticalComponenet += (0.35f / 2))
        {
            rayDirection.x = (monsterSR.flipX ? 1 : -1) * horizontalViewRange;
            rayDirection.y = verticalComponenet;

            if (Physics2D.Raycast(transform.position - mushroomHeightOffset , 
                rayDirection, rayDirection.magnitude , playerLayer))
            {
                attackMove = true;
                break;
            }
        }
    }

    public void IsAttacking()
    {
        attackMove = false;
        attacking = true;
    } //For AHBMushroom Script

                                    private const float idleAfterAttack = 0.5f;
                                    private float afterAttackIdleTimeTracker = 0f;
                                    private bool idleAfterAttackBool = false;
    private void AfterAttackIdle()
    {
        afterAttackIdleTimeTracker += Time.deltaTime;

        if (afterAttackIdleTimeTracker >= idleAfterAttack)
        {
            idleAfterAttackBool = false;
            afterAttackIdleTimeTracker = 0f;
        }

        mushroomRB.linearVelocity = Vector2.zero;
    }
    private void IsNotAttacking() //For animator use.
    {
        attacking = false;
        idleAfterAttackBool = true;
    }
    //--------------------------------------------------| ATTACK FUNCTIONS
    //--------------------------------------------------| HEALTH FUNCTIONS
    private bool dead = false;
    public override void LoseHealth(int healthLost)
    {
        monsterHealth -= healthLost;

        if (monsterHealth <= 0)
        {
            dead = true;
            attackMove = false;
        }
    }
    protected override void Death()
    {
        Destroy(gameObject);
    }
    //--------------------------------------------------| HEALTH FUNCTIONS
    //--------------------------------------------------| STATE SETTING FUNCTIONS
    protected override void SetState()
    {
        if (dead)
            mushroomState = MushroomStates.Death;

        else if (idleAfterAttackBool)
            mushroomState = MushroomStates.Idle;

        else if (attacking)
            mushroomState = MushroomStates.Attack;

        else if (Mathf.Abs(mushroomRB.linearVelocityX) > 0.01f)
            mushroomState = MushroomStates.Move;

        else if (mushroomRB.linearVelocityX <= 0.01f)
            mushroomState = MushroomStates.Idle;

        DirectionSetting();
    }

    protected override void StateEffects()
    {
        if (mushroomState == MushroomStates.Death)
        {
            monsterAR.SetBool("Dead", true);
            monsterAR.SetBool("Attack", false);
            monsterAR.SetBool("Moving", false);
            monsterAR.SetBool("Idle", false);
        }

        else if (mushroomState == MushroomStates.Attack)
        {
            monsterAR.SetBool("Attack", true);
            monsterAR.SetBool("Moving", false);
            monsterAR.SetBool("Idle", false);
        }

        else if (mushroomState == MushroomStates.Move)
        {
            monsterAR.SetBool("Attack", false);
            monsterAR.SetBool("Moving", true);
            monsterAR.SetBool("Idle", false);
        }
        else if (mushroomState == MushroomStates.Idle)
        {
            monsterAR.SetBool("Attack", false);
            monsterAR.SetBool("Moving", false);
            monsterAR.SetBool("Idle", true);
        }
    }

    private Vector2 hitBoxLocation = new Vector2(0 , hitBoxLocationY);
    private const float hitBoxLocationY = -0.16f;
    private void DirectionSetting()
    {
        if (Mathf.Abs(mushroomRB.linearVelocityX) <= 0.01f)
            monsterSR.flipX = monsterSR.flipX;
        
        else
        {
            monsterSR.flipX = (mushroomRB.linearVelocityX < 0 ? false : true);

            hitBoxLocation.x = (monsterSR.flipX ? Mathf.Abs(attackCollider.offset.x) : -Mathf.Abs(attackCollider.offset.x));
            attackCollider.offset = hitBoxLocation;
        }
    }
    //--------------------------------------------------| STATE SETTING FUNCTIONS
}
