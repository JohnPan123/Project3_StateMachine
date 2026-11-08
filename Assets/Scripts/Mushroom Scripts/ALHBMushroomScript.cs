using UnityEngine;

public class ALHBMushroomScript : MonoBehaviour
{
    [SerializeField] private GameObject mushroom;
    [SerializeField] private GameObject attackingLandingHitBox;

    private void OnTriggerStay2D(Collider2D collision)
    {
        attackingLandingHitBox.SetActive(false);
    }

    public GameObject GetParent => mushroom;
}
