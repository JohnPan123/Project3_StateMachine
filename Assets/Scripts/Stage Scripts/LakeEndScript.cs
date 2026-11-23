using UnityEngine;

public class LakeEndScript : StageObjectScript
{
    [SerializeField] private PlayerScript playerScript;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == GenericScript.PlayerName)
        {
            GameOverTextScript.victory = false;
            ChangeScene.ChangeSceneFunction("Game End Scene");
        }
        else 
            Destroy(collision.gameObject);
    }
}