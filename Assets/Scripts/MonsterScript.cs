using UnityEngine;

public abstract class MonsterScript : MonoBehaviour
{
    [SerializeField] protected LayerMask playerLayer;
    [SerializeField] protected BoxCollider2D attackBreathCollider;

    protected GameObject player;
    protected SpriteRenderer monsterSR;
    protected Animator monsterAR;

    protected int monsterHealth;
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
