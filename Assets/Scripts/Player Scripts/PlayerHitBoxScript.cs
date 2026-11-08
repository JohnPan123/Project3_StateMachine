using UnityEngine;

public class PlayerHitBoxScript : MonoBehaviour
{
    private const int damageDealt = 1;
    private GameObject collidedObject;
    private void OnTriggerEnter2D(Collider2D collision) 
    {
        collidedObject = collision.gameObject;

        //I do not need to check what I am colliding with. The Layer collision matrix
        //is set to only allow collisions for the playerHitBoxLayer with the enemy layer.
        //Also, I am using polymorphism here to access the functions in the two different 
        //enemy types using the type that they derive from using one call.
        collision.gameObject.GetComponent<MonsterScript>().LoseHealth(damageDealt);
    }
}