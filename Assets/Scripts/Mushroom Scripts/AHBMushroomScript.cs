using UnityEngine;

public class AHBMushroomScript : MonoBehaviour
{
    [SerializeField] private MushroomScript mushroomScript;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Can only collide with player due to layer collision matrix.
        //Cannot touch anything else. No need to check.
        Debug.Log(true);

        mushroomScript.IsAttacking();
    }
}
