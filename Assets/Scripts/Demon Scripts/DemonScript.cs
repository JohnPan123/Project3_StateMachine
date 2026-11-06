using UnityEngine;

public class DemonScript : MonsterScript
{
    private BaseMoveStates moveState;
    private DemonStates demonState;
    private enum BaseMoveStates
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
        Death = 2,
        BreathAttack = 3
    }

    void Start()
    {
        player = GameObject.Find(PlayerScript.playerName);

        monsterAR = GetComponent<Animator>();
        monsterSR = GetComponent<SpriteRenderer>();

        attackBreathCollider.enabled = false;

        demonState = DemonStates.Idle;
        moveState = BaseMoveStates.Idle;

        monsterHealth = 3;
    }

    void Update()
    {
        DemonMove();
        
        if (!attacking)
            MonsterDetectAttack();

        SetState();
        StateEffects();
    }

    //--------------------------------------------------| MOVEMENT FUNCTIONS
    private void DemonMove()
    {
        if (dead) ;

        else if (!attacking)
            MonsterBaseMove();

        else if (attacking)
            MonsterAttackMove();
    }


                                private Vector3 moveVector = new Vector3(demonSpeed, 0f, 0f);
                                private const float demonSpeed = 3.5f;
                                [SerializeField] private float timeToChangeMove = 1f;
                                private float timeTillMoveChange = 0f;
    protected override void MonsterBaseMove()
    {
        if (attackBreathCollider.enabled)
            attackBreathCollider.enabled = false;

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
                break;
            case BaseMoveStates.MoveLeft:
                transform.Translate(-moveVector * Time.deltaTime);
                break;
            case BaseMoveStates.IdleTwo:
                break;
            case BaseMoveStates.MoveRight:
                transform.Translate(moveVector * Time.deltaTime);
                break;
        }
    }


                                private Vector3 leftAttackOffset = new Vector3(-0.5f, 0.5f, 0);
                                private Vector3 rightAttackOffset = new Vector3(0.5f, 0.5f, 0);
                                private const float demonAttackSpeed = 4.5f;
    protected override void MonsterAttackMove()
    {
        if (!attackBreathCollider.enabled && monsterAR.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Demon Attack Breath")
            attackBreathCollider.enabled = true;

        if (transform.position.x < player.transform.position.x)
        {
            moveVector = (player.transform.position - transform.position + leftAttackOffset).normalized;
            moveState = BaseMoveStates.MoveRight;
        }
        else
        {
            moveVector = (player.transform.position - transform.position + rightAttackOffset).normalized;
            moveState = BaseMoveStates.MoveLeft;
        }

        transform.Translate(moveVector * demonAttackSpeed * Time.deltaTime);
    }
    //--------------------------------------------------| MOVEMENT FUNCTIONS
    //--------------------------------------------------| ATTACKING FUNCTIONS


                                private const float verticalViewRange = 1f;
                                private const float horizontalViewRange = 3f;
                                private Vector2 rayDirection = new Vector2(0 , 0);
                                private bool attacking = false;
    protected override void MonsterDetectAttack()
    {
        for (float verticalComponenet = 0f ; verticalComponenet <= verticalViewRange ; verticalComponenet += 0.2f)
        {
            rayDirection.x = (monsterSR.flipX ? 1 : -1) * horizontalViewRange;
            rayDirection.y = -verticalComponenet;

            if (Physics2D.Raycast(transform.position, rayDirection, rayDirection.magnitude, playerLayer))
            {
                attacking = true;
                break;
            }
        }
    }
    //--------------------------------------------------| ATTACKING FUNCTIONS
    //--------------------------------------------------| HEALTH FUNCTIONS
                                private bool dead = false;
                                private Vector2 deathSpriteOffset = new Vector2(0.7f , 0);
    public override void LoseHealth(int healthLost)
    {
        monsterHealth -= healthLost;

        if (monsterHealth <= 0)
        {
            if (moveState == BaseMoveStates.MoveLeft)
                transform.Translate(deathSpriteOffset);
            
            else if (moveState == BaseMoveStates.MoveRight)
                transform.Translate(-deathSpriteOffset);

            attackBreathCollider.enabled = false;
            dead = true;
            attacking = false;
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
            demonState = DemonStates.Death;
        else if (attacking)
            demonState = DemonStates.BreathAttack;

        if (moveState == BaseMoveStates.MoveLeft)
            monsterSR.flipX = false;
        else if (moveState == BaseMoveStates.MoveRight)
            monsterSR.flipX = true;
    }

    protected override void StateEffects()
    {
        if (demonState == DemonStates.Death)
            monsterAR.SetBool("Dead", true);
        
        else if (demonState == DemonStates.BreathAttack)
            monsterAR.SetBool("Attacking" , true);
    }
    //--------------------------------------------------| STATE SETTING FUNCTIONS
}
