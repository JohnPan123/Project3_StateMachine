using TMPro;
using UnityEngine;

public class GameOverTextScript : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI gameEndText;
    public static bool victory = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (victory)
            gameEndText.text = "Victory";
        else
            gameEndText.text = "You Lost";
    }
}
