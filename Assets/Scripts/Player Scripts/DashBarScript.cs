using UnityEngine;
using UnityEngine.UI;

public class DashBarScript : MonoBehaviour
{
    [SerializeField] private PlayerScript playerScript;
    [SerializeField] private Slider slider;

    // Update is called once per frame
    void Update()
    {
        slider.value = playerScript.DashBarPercentToFill;
    }
}
