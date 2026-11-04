using UnityEngine;

public class AHBDemonScript : MonoBehaviour
{
    [SerializeField] private Animator demonAR;
    private const float breathAttackDamage = 0.5f;
    private void OnTriggerStay2D(Collider2D collision)
    {
        GameObject objectCollidedWith = collision.gameObject;

        Debug.Log(true);

        if (objectCollidedWith.CompareTag(PlayerScript.playerName))
            objectCollidedWith.GetComponent<PlayerScript>().HealthLost(breathAttackDamage);
    }
}
