using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [SerializeField] private GameObject player;

                                    private Vector3 nextPosition = new Vector2();
                                    private const float playerCameraOffset = -7.78f / 2;
                                    private const int cameraDistanceFromLevel = -10;
    private void Start()
    {
        nextPosition.x = -4.68f;
        nextPosition.y = 0f;
        nextPosition.z = cameraDistanceFromLevel;
    }


                                    private const float leftBoundCameraMeasure = -8.67f;
                                    private const float rightBoundCameraMeasure = 47f;
                                    private float playerXPosition;
    // Update is called once per frame
    void Update()
    {
        playerXPosition = player.transform.position.x;

        if ((playerXPosition >= leftBoundCameraMeasure) && (playerXPosition <= rightBoundCameraMeasure))
            nextPosition.x = (player.transform.position.x - playerCameraOffset);

        transform.position = nextPosition;
    }
}
