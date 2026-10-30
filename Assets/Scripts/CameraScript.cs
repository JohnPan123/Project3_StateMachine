using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [SerializeField] private GameObject player;

    private Vector3 nextPosition = new Vector2();

    private const float playerCameraOffset = -7.78f / 2;
    private const int cameraDistanceFromLevel = -10;
    private void Start()
    {
        nextPosition.y = 0f;
        nextPosition.z = -10;
    }
    // Update is called once per frame
    void Update()
    {
        nextPosition.x = player.transform.position.x - playerCameraOffset;
        
        transform.position = nextPosition;
    }
}
