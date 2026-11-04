using UnityEngine;

public class AttackHitBoxScript : MonoBehaviour
{
    private const float breathAttackDamage = 0.5f;
    private void OnTriggerStay2D(Collider2D collision)
    {
        GameObject objectCollidedWith = collision.gameObject;

        if (objectCollidedWith.CompareTag(PlayerScript.playerName))
        {
            objectCollidedWith.GetComponent<PlayerScript>().HealthLost(breathAttackDamage);
            Debug.Log(true);
        }
    }
}
