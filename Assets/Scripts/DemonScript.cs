using UnityEngine;

public class DemonScript : MonoBehaviour
{
    [SerializeField] private LayerMask playerLayer;
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
        demonAR = GetComponent<Animator>();
        demonSR = GetComponent<SpriteRenderer>();

        demonState = DemonStates.Idle;
    }

    void Update()
    {
        if (!attacking)
            DemonMove();
        
        DemonDetectAttack();

        SetState();
        StateEffects();
    }
    private const float demonSpeed = 3.5f;
    private Vector3 moveVector = new Vector3(demonSpeed , 0f , 0f);
    private float timeToChangeMove = 1f;
    private float timeTillMoveChange = 0f;
    private MoveStates moveState = MoveStates.Idle;
    private void DemonMove()
    {
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
    //--------------------------------------------------| STATE SETTING FUNCTIONS
    private void SetState()
    {
        if (attacking)
            demonState = DemonStates.BreathAttack;
        else
        {
            if (moveState == MoveStates.MoveLeft)
                demonSR.flipX = false;
            else if (moveState == MoveStates.MoveRight)
                demonSR.flipX = true;
        }
    }

    private void StateEffects()
    {
        if (demonState == DemonStates.BreathAttack)
            demonAR.SetBool("Attacking" , true);
    }
    //--------------------------------------------------| STATE SETTING FUNCTIONS
}
