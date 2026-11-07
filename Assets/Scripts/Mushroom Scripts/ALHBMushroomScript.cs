using UnityEngine;

public class PHBMushroomScript : MonoBehaviour
{
    [SerializeField] private GameObject physicsHB;

    private void OnTriggerStay2D(Collider2D collision)
    {
        physicsHB.SetActive(false);
    }
}
