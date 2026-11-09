using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public static void ChangeSceneFunction(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
