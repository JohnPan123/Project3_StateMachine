using UnityEngine;

public class PlayerHitBoxScript : MonoBehaviour
{
    private const int damageDealt = 1;
    private void OnTriggerEnter2D(Collider2D collision) 
    {
        //No need to check what I am colliding with.
        //The player hit box layer can only contact the enemy layer
        //due to the player collision matrix.
        collision.gameObject.GetComponent<DemonScript>().LoseHealth(damageDealt);
    }
}