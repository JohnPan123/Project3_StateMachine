using UnityEngine;

public class PlatformScript : MonoBehaviour
{
    [SerializeField] private GameObject player;
    private BoxCollider2D platformBox;

    void Start()
    {

        platformBox = GetComponent<BoxCollider2D>();

        platformBox.enabled = false;
    }

    void Update()
    {
        PlatformActivation();
    }

    private const float playerHeightAbovePlatform = 0.9315f;
    private void PlatformActivation()
    {
        if (player.transform.position.y > (transform.position.y + playerHeightAbovePlatform))
            platformBox.enabled = true;
        else
            platformBox.enabled = false;
    }
}
