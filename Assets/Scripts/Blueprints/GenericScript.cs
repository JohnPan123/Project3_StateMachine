using UnityEngine;

public class GenericScript : MonoBehaviour
{
    private static string playerName;
    public static string PlayerName => playerName;
    private void Start()
    {
        playerName = gameObject.name;
    }
}
