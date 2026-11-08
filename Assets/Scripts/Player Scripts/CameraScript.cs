using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [SerializeField] private GameObject player;

    private Vector3 nextPosition = new Vector2();
    private const float playerCameraOffset = -7.78f / 2;
    private const int cameraDistanceFromLevel = -10;
    private void Start()
    {
        nextPosition.x = 0.3f;
        nextPosition.y = 0f;
        nextPosition.z = cameraDistanceFromLevel;
    }

    private const float leftBoundCameraMeasure = -3.58f;
    // Update is called once per frame
    void Update()
    {
        if (player.transform.position.x >= leftBoundCameraMeasure)
            nextPosition.x = player.transform.position.x - playerCameraOffset;

        transform.position = nextPosition;
    }
}
