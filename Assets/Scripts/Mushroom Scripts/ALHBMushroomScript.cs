using UnityEngine;

public class PHBMushroomScript : MonoBehaviour
{
    [SerializeField] private GameObject attackingLandingHitBox;

    private void OnTriggerStay2D(Collider2D collision)
    {
        attackingLandingHitBox.SetActive(false);
    }
}
