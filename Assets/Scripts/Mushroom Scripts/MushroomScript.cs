using UnityEngine;

public class MushroomScript : MonsterScript
{
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

        mushroomState = MushroomStates.Idle;
        moveState = BaseMoveStates.Idle;
    }

    void Update()
    {
        MonsterBaseMove();

        SetState();
        StateEffects();
    }
    //--------------------------------------------------| MOVEMENT FUNCTIONS
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

    private bool attackMove = false;
    protected override void MonsterAttackMove()
    {
        throw new System.NotImplementedException();
    }
    //--------------------------------------------------| MOVEMENT FUNCTIONS
    //--------------------------------------------------| ATTACK FUNCTIONS
    private bool attacking = false;
    protected override void MonsterDetectAttack()
    {
        throw new System.NotImplementedException();
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
        if (dead)
            mushroomState = MushroomStates.Death;
        else if (attackMove)
            mushroomState = MushroomStates.AttackMove;
        else if (attacking)
            mushroomState = MushroomStates.Attack;

        if (moveState == BaseMoveStates.MoveLeft)
            monsterSR.flipX = false;
        else if (moveState == BaseMoveStates.MoveRight)
            monsterSR.flipX = true;
    }

    protected override void StateEffects()
    {
        if (mushroomState == MushroomStates.Death)
            monsterAR.SetBool("Dead", true);

        else if (mushroomState == MushroomStates.Attack)
            monsterAR.SetBool("Attacking", true);

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
    //--------------------------------------------------| STATE SETTING FUNCTIONS
}
