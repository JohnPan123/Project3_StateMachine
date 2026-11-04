using UnityEngine;

public class PlayerHitBoxScript : MonoBehaviour
{
    private GameObject collisionObject;
    private const int enemyLayer = 10;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        collisionObject = collision.gameObject;

        if (collisionObject.layer == enemyLayer)
            Destroy(collisionObject);
    }
}
