using UnityEngine;

public class ALHBMushroomScript : MonoBehaviour
{
    [SerializeField] private GameObject mushroom;
    [SerializeField] private GameObject attackingLandingHitBox;
    private const float damageDone = 10f;

    private void OnTriggerStay2D(Collider2D collision)
    {
        //This gameobject can only collide with the player. No need to check anything.
        collision.gameObject.GetComponent<PlayerScript>().HealthLost(damageDone);
        attackingLandingHitBox.SetActive(false);
    }

    public GameObject GetParent => mushroom;
}
