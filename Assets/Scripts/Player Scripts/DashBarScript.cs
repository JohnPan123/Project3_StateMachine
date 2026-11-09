using UnityEngine;
using UnityEngine.UI;

public class DashBarScript : MonoBehaviour
{
    [SerializeField] private PlayerScript playerScript;
    [SerializeField] private Image colorForBar;
    [SerializeField] private Slider slider;

    private float valueThisFrame;

    // Update is called once per frame
    void Update()
    {
        valueThisFrame = playerScript.DashBarPercentToFill;
        slider.value = valueThisFrame;

        if (valueThisFrame < 1f)
            colorForBar.color = Color.black;
        else
            colorForBar.color = Color.blue;
    }
}
