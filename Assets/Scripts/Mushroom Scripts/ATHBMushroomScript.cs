using UnityEngine;

public class ATHBMushroomScript : MonoBehaviour
{
    [SerializeField] private MushroomScript mushroomScript;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Can only collide with player due to layer collision matrix.
        //Cannot touch anything else. No need to check.
        mushroomScript.IsAttacking();
    }
}
