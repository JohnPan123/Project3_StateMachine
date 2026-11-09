using UnityEngine;

public class LakeEndScript : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == PlayerScript.playerName)
        {
            GameOverTextScript.victory = false;
            ChangeScene.ChangeSceneFunction("Game End Scene");
        }
        else 
            Destroy(collision.gameObject);
    }
}