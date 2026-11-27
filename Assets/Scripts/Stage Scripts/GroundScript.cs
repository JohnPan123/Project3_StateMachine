using UnityEngine;

public class GroundScript : StageObjectScript
{
    [SerializeField] private EdgeCollider2D platformBox;

    void Update() { PlatformActivation(); }

    private const float playerIsHighEnoughFromPlatformHeight = 0.83f;
    private float deactivationHeight = 0.01f;
    private void PlatformActivation()
    {
        //Exclude layers ovverides include layers so I do not need to handle that one.
        if (player.transform.position.y > (transform.position.y + playerIsHighEnoughFromPlatformHeight))
            platformBox.excludeLayers = noLayer;

        else if (player.transform.position.y < (transform.position.y - deactivationHeight))
            platformBox.excludeLayers = playerLayer;
    }
}
