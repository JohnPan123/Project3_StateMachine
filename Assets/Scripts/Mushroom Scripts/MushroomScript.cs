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
        Death = 2,
        BreathAttack = 3
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

    }
    public override void LoseHealth(int healthLost)
    {
        throw new System.NotImplementedException();
    }

    protected override void Death()
    {
        throw new System.NotImplementedException();
    }

    protected override void MonsterAttackMove()
    {
        throw new System.NotImplementedException();
    }

    protected override void MonsterBaseMove()
    {
        throw new System.NotImplementedException();
    }

    protected override void MonsterDetectAttack()
    {
        throw new System.NotImplementedException();
    }

    protected override void SetState()
    {
        throw new System.NotImplementedException();
    }

    protected override void StateEffects()
    {
        throw new System.NotImplementedException();
    }

    
}
