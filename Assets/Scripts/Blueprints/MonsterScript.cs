using UnityEngine;

public abstract class MonsterScript : StageEntityScript
{
    [SerializeField] protected LayerMask playerLayer;
    [SerializeField] protected BoxCollider2D attackCollider;
    [SerializeField] protected GameObject player;

    //--------------------------------------------------| MOVEMENT FUNCTIONS
    protected abstract void MonsterBaseMove();
    protected abstract void MonsterAttackMove();
    //--------------------------------------------------| MOVEMENT FUNCTIONS
    //--------------------------------------------------| ATTACKING FUNCTIONS
    protected abstract void MonsterDetectAttack();
    //--------------------------------------------------| ATTACKING FUNCTIONS
    //--------------------------------------------------| HEALTH FUNCTIONS
    public abstract void LoseHealth(int healthLost);

    protected abstract void Death();
    //--------------------------------------------------| HEALTH FUNCTIONS
    //--------------------------------------------------| STATE SETTING FUNCTIONS
    protected abstract void SetState();

    protected abstract void StateEffects();
    //--------------------------------------------------| STATE SETTING FUNCTIONS
}
