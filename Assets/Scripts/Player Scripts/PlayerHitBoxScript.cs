using UnityEngine;

public class PlayerHitBoxScript : MonoBehaviour
{
    private const int damageDealt = 1;
    private GameObject collidedObject;
    private void OnTriggerEnter2D(Collider2D collision) 
    {
        collidedObject = collision.gameObject;

        if (collidedObject.GetComponent<DemonScript>() != null)
            collision.gameObject.GetComponent<DemonScript>().LoseHealth(damageDealt);

        else if (collidedObject.GetComponent<MushroomScript>() != null)
            collision.gameObject.GetComponent<MushroomScript>().LoseHealth(damageDealt);

    }
}