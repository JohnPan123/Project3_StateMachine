using UnityEngine;

public class StageEndScript : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameOverTextScript.victory = true;
        ChangeScene.ChangeSceneFunction("Game End Scene");
    }
}
