using UnityEngine;

public class MushroomScript : MonsterScript
{
    [SerializeField] private int playerAttackTriggerColliderLayer;
    [SerializeField] private GameObject attackLanderHB;

    private Rigidbody2D mushroomRB;
    private MushroomStates mushroomState;
    private enum MushroomStates
    {
        Idle = 0,
        Move = 1,
        Attack = 2,
        TookHit = 3,
        Death = 4
    }
    void Start()
    {
        player = GameObject.Find(PlayerScript.playerName);

        monsterAR = GetComponent<Animator>();
        monsterSR = GetComponent<SpriteRenderer>();
        mushroomRB = GetComponent<Rigidbody2D>();

        mushroomState = MushroomStates.Idle;
        monsterHealth = 3;
    }

    private void FixedUpdate()
    {
        if (!(idleAfterAttackBool || idleAfterFlinchingBool || attacking || flinching || dead))
            MushroomMove();
    }
    void Update()
    {
        //DebugFunction();
        if (!dead)
        {
            if (attacking) ;
            else if (idleAfterFlinchingBool)
                AfterFlinchingIdle();
            else if (idleAfterAttackBool)
                AfterAttackIdle();
            else
                MonsterDetectAttack();
        }

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
                                private const float attackOffset = 0.5f;
    protected override void MonsterAttackMove()
    {
        if (transform.position.x < (player.transform.position.x - attackOffset))
        {
            moveVectorAttack.x = mushroomAttackSpeed;
            moveVectorAttack.y = mushroomRB.linearVelocityY;

            mushroomRB.linearVelocity = moveVectorAttack;
        }

        else if (transform.position.x > (player.transform.position.x + attackOffset))
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

                                private bool needToSetAttackTriggerForAnimator = true;
    public void IsAttacking()
    {
        attackCollider.gameObject.SetActive(false);

        attackMove = false;
        attacking = true;
        needToSetAttackTriggerForAnimator = true;
        //Sets attack direction based on player direction
        monsterSR.flipX = ((player.transform.position.x > transform.position.x) ? true : false); 
        SetHitBoxBasedOnDirection();
    } //For AHBMushroom Script

    public void EnableHitBoxForAttack() //For animator
    {
        attackLanderHB.SetActive(true);
        attackLanderHB.GetComponent<BoxCollider2D>().offset = attackCollider.offset;
    }
    public void DisableHitBoxForAttack() //For animator and animator functions
    {
        if (attackLanderHB.activeSelf)
        {
            attackLanderHB.GetComponent<BoxCollider2D>().offset = Vector2.zero;
            attackLanderHB.SetActive(false);
        }
    }

                                    private float afterAttackIdleTimeTracker = 0f;
                                    private bool idleAfterAttackBool = false;
    private void IsDoneAttacking() //For animator use.
    {
        attacking = false;
        idleAfterAttackBool = true;
        afterAttackIdleTimeTracker = 0f;

        DisableHitBoxForAttack();
    }

                                    private const float idleAfterAttackTime = 1.3f;
    private void AfterAttackIdle()
    {
        afterAttackIdleTimeTracker += Time.deltaTime;

        if (afterAttackIdleTimeTracker >= idleAfterAttackTime)
        {
            attackCollider.gameObject.SetActive(true);
            idleAfterAttackBool = attackMove = false;
        }

        mushroomRB.linearVelocity = Vector2.zero;
    }
    //--------------------------------------------------| ATTACK FUNCTIONS
    //--------------------------------------------------| RECEIVE ATTACK FUNCTIONS
                                    private bool idleAfterFlinchingBool = false;
    private void IsDoneFlinching()
    {
        flinching = false;
        idleAfterFlinchingBool = true;
        afterFlinchingIdleTimeTracker = 0f;
        //The disable hit box is run at the start of the animation event. Not need to run it again.
    }

                                    private float afterFlinchingIdleTimeTracker = 0f;
                                    private const float idleAfterFlinchingTime = 1.3f;
    private void AfterFlinchingIdle()
    {
        afterFlinchingIdleTimeTracker += Time.deltaTime;

        if (afterFlinchingIdleTimeTracker >= idleAfterFlinchingTime)
        {
            attackCollider.gameObject.SetActive(true);
            idleAfterFlinchingBool = attackMove = false;
        }

        mushroomRB.linearVelocity = Vector2.zero;
    }
    private bool flinching, needToRegisterReceivedHitTriggerForAnimator = false;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject objectOfTriggerCollision = collision.gameObject;

        //Sets direction based only incoming attack direction
        monsterSR.flipX = ((player.transform.position.x > transform.position.x) ? true : false);

        if ((!dead) && (objectOfTriggerCollision.layer == playerAttackTriggerColliderLayer))
        {
            flinching = needToRegisterReceivedHitTriggerForAnimator = true;

            attacking = attackMove = idleAfterAttackBool = idleAfterFlinchingBool = false;
            afterFlinchingIdleTimeTracker = afterAttackIdleTimeTracker = 0f;
        }

    }

    //--------------------------------------------------| RECEIVE ATTACK FUNCTIONS
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


        else if (idleAfterAttackBool || idleAfterFlinchingBool)
            mushroomState = MushroomStates.Idle;

        else if (flinching)
            mushroomState = MushroomStates.TookHit;

        else if (attacking)
            mushroomState = MushroomStates.Attack;

        else if (Mathf.Abs(mushroomRB.linearVelocityX) > 0.1f)
            mushroomState = MushroomStates.Move;

        else if (mushroomRB.linearVelocityX <= 0.1f)
            mushroomState = MushroomStates.Idle;

        DirectionSetting();
    }

    protected override void StateEffects()
    {
        if (mushroomState == MushroomStates.Death)
            monsterAR.SetTrigger("Dead");

        else if (mushroomState == MushroomStates.TookHit)
        {
            if (needToRegisterReceivedHitTriggerForAnimator)
            {
                monsterAR.SetTrigger("Took Damage");
                needToRegisterReceivedHitTriggerForAnimator = false;
            }
        }

        else if (mushroomState == MushroomStates.Attack)
        {
            if (needToSetAttackTriggerForAnimator)
            {
                monsterAR.SetTrigger("Attack");
                needToSetAttackTriggerForAnimator = false;
            }
        }

        else if (mushroomState == MushroomStates.Move)
            monsterAR.SetTrigger("Moving");

        else if (mushroomState == MushroomStates.Idle)
            monsterAR.SetTrigger("Idle");
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
            SetHitBoxBasedOnDirection();
        }
    }

    private const float colliderXOffset = 0.11f;
    private void SetHitBoxBasedOnDirection()
    {
        hitBoxLocation.x = (monsterSR.flipX ? colliderXOffset : -colliderXOffset);
        attackCollider.offset = hitBoxLocation;
    }
    //--------------------------------------------------| STATE SETTING FUNCTIONS
    private void DebugFunction()
    {
        Debug.Log($"Attacking: {attacking} || AttackMove: {attackMove} || TookHit: {flinching} || State: {mushroomState}");
    }
}
