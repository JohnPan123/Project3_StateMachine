using UnityEngine;

public class DemonScript : MonoBehaviour
{
    [SerializeField] private LayerMask playerLayer;
    private Animator demonAR;
    private DemonStates demonState;
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

        demonState = DemonStates.Idle;
    }

    void Update()
    {
        DemonMove();
        DemonDetectAttack();

        SetState();
        StateEffects();
    }
    private const float demonSpeed = 3f;
    private void DemonMove()
    {

    }
    private const float verticalViewRange = 1f;
    private const float horizontalViewRange = 3f;
    private Vector2 rayDirection = new Vector2(0, 0);
    private bool attacking = false;
    private void DemonDetectAttack()
    {
        for (float verticalComponenet = 0f ; verticalComponenet <= verticalViewRange ; verticalComponenet += 0.2f)
        {
            rayDirection.x = -horizontalViewRange;
            rayDirection.y = -verticalComponenet;

            //Debug.DrawRay(transform.position, rayDirection, Color.red);

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
        //else if ()

    }

    private void StateEffects()
    {
        if (demonState == DemonStates.BreathAttack)
            demonAR.SetBool("Attacking" , true);
    }
    //--------------------------------------------------| STATE SETTING FUNCTIONS
}
