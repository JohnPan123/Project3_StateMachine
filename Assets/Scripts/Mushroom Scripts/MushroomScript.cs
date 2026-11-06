using UnityEditor;
using UnityEngine;

public class MushroomScript : MonsterScript
{
    private Rigidbody2D mushroomRB;
    private MushroomStates mushroomState;
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
    }

    private void FixedUpdate()
    {
        if (!idleAfterAttackBool)
            MushroomMove();
    }
    void Update()
    {
        if (!idleAfterAttackBool)
            MonsterDetectAttack();
        else
            AfterAttackIdle();

        SetState();
        StateEffects();
    }
    //--------------------------------------------------| MOVEMENT FUNCTIONS
    private void MushroomMove()
    {
        if (idleAfterAttackBool) ;
        else if (attacking) ;
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
                                private float distanceTravelledTracker = 0f;
                                private float idleTimeTracker = 0f;
                                private int movePhase = 0;
    protected override void MonsterBaseMove()
    {
        distanceTravelledTracker += Mathf.Abs(mushroomRB.linearVelocityX) * physicsTickTime;

        if ((movePhase % 2) == 0)
            idleTimeTracker += Time.deltaTime;

        //Debug.Log("Distance: " + (distanceTravelledTracker >= distanceToChangeMove));
        //Debug.Log("Time: " + (idleTimeTracker >= idleTime));

        if ((distanceTravelledTracker >= distanceToChangeMove) || idleTimeTracker >= idleTime)
        {
            if ((movePhase + 1) == 4)
                movePhase = 0;
            else
                movePhase++;

            idleTimeTracker = 0f;
            distanceTravelledTracker = 0f;
        }

        switch (movePhase)
        {
            case 0:
                moveVector.x = 0f;
                moveVector.y = mushroomRB.linearVelocityY;

                mushroomRB.linearVelocity = Vector2.zero;
                break;
            case 1:
                moveVector.x = -monsterSpeed;
                moveVector.y = mushroomRB.linearVelocityY;

                mushroomRB.linearVelocity = moveVector;
                break;
            case 2:
                moveVector.x = 0f;
                moveVector.y = mushroomRB.linearVelocityY;

                mushroomRB.linearVelocity = Vector2.zero;
                break;
            case 3:
                moveVector.x = monsterSpeed;
                moveVector.y = mushroomRB.linearVelocityY;

                mushroomRB.linearVelocity = moveVector;
                break;
        }
    }


                                private bool attacking = false;
                                private Vector3 moveVectorAttack = new Vector3(0 , 0 , 0);
                                private const float mushroomAttackSpeed = 4.5f;
    protected override void MonsterAttackMove()
    {
        if (transform.position.x <= player.transform.position.x)
        {
            moveVectorAttack.x = mushroomAttackSpeed;
            moveVectorAttack.y = mushroomRB.linearVelocityY;

            mushroomRB.linearVelocity = (Vector2)moveVectorAttack;
        }

        else if (transform.position.x > player.transform.position.x)
        {
            moveVectorAttack.x = -mushroomAttackSpeed;
            moveVectorAttack.y = mushroomRB.linearVelocityY;

            mushroomRB.linearVelocity = (Vector2)moveVectorAttack;
        }
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

    public void IsAttacking()
    {
        attacking = true;
    }

    private const float idleAfterAttack = 0.5f;
    private float trackForAfterAttackTime = 0f;
    private bool idleAfterAttackBool = false;
    private void AfterAttackIdle()
    {
        trackForAfterAttackTime += Time.deltaTime;

        if (trackForAfterAttackTime >= idleAfterAttack)
        {
            idleAfterAttackBool = false;
            trackForAfterAttackTime = 0f;
        }
    }

    private void IsNotAttacking() //For animator use.
    {
        trackForAfterAttackTime = 0f;

        attacking = false;
        attackMove = false;
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
        DirectionSetting();

        if (dead)
            mushroomState = MushroomStates.Death;

        else if (idleAfterAttackBool)
            mushroomState = MushroomStates.Idle;

        else if (attacking)
            mushroomState = MushroomStates.Attack;

        else if (attackMove)
            mushroomState = MushroomStates.AttackMove;

        else if (Mathf.Abs(mushroomRB.linearVelocityX) > 0.01f)
            mushroomState = MushroomStates.Move;

        else if (mushroomRB.linearVelocityX <= 0.01f)
            mushroomState = MushroomStates.Idle;

    }

    protected override void StateEffects()
    {
        if (mushroomState == MushroomStates.Death)
            monsterAR.SetBool("Dead", true);

        else if (mushroomState == MushroomStates.Attack)
        {
            monsterAR.SetBool("Attack", true);
            monsterAR.SetBool("AttackMove", false);
        }

        else if (mushroomState == MushroomStates.AttackMove)
        {
            monsterAR.SetBool("Attack", false);
            monsterAR.SetBool("AttackMove", true);
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

    private void DirectionSetting()
    {
        if (Mathf.Abs(mushroomRB.linearVelocityX) <= 0.01f)
            monsterSR.flipX = monsterSR.flipX;
        
        else
            monsterSR.flipX = (mushroomRB.linearVelocityX < 0 ? false : true);
    }
    //--------------------------------------------------| STATE SETTING FUNCTIONS
}
