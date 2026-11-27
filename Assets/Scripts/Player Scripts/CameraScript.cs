using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [SerializeField] private GameObject player;

    [SerializeField] private Vector3 nextPosition = new Vector2();
                                    private const float playerCameraOffset = -7.78f / 2;
                                    private const float leftBoundCameraMeasure = -7.87f;
                                    private const float rightBoundCameraMeasure = 47f;
                                    private float playerXPosition;
    void Update()
    {
        playerXPosition = player.transform.position.x;

        if ((playerXPosition >= leftBoundCameraMeasure) && (playerXPosition <= rightBoundCameraMeasure))
            nextPosition.x = (player.transform.position.x - playerCameraOffset);

        transform.position = nextPosition;
    }
}
