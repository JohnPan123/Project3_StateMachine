using UnityEngine;

public class PlatformScript : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask noLayer;
    private BoxCollider2D platformBox;

    void Start()
    {
        platformBox = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        PlatformActivation();
    }

    private const float playerIsHighEnoughFromPlatformHeight = 0.9315f;
    private void PlatformActivation()
    {
        if (player.transform.position.y > (transform.position.y + playerIsHighEnoughFromPlatformHeight))
        {
            platformBox.includeLayers = playerLayer;
            platformBox.excludeLayers = noLayer;

        }
        else
        {
            platformBox.includeLayers = noLayer;
            platformBox.excludeLayers = playerLayer;
        }
    }
}
