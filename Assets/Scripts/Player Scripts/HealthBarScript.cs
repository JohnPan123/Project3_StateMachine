using UnityEngine;
using UnityEngine.UI;

public class HealthBarScript : MonoBehaviour
{
    [SerializeField] private GameObject player;
    private PlayerScript playerScript;

    private Slider healthSlider;
    [SerializeField] private Image sliderThatIsColored;

    private const float playerMaxHeatlh = 100f;
    private float playerHealth;
    private bool updatedMidHealthColor = false;
    private const float midHealthValue = 50f;
    private bool updatedLowHealthColor = false;
    private const float lowHealthValue = 20f;

    void Start()
    {
        playerScript = player.GetComponent<PlayerScript>();

        healthSlider = GetComponent<Slider>();
        healthSlider.value = playerMaxHeatlh;
    }
    void Update()
    {
        playerHealth = playerScript.GetPlayerHealth;
        healthSlider.value = playerHealth;

        if (!updatedMidHealthColor && (playerHealth <= midHealthValue))
        {
            sliderThatIsColored.color = Color.yellow;
            updatedMidHealthColor = true;
        }

        else if (!updatedLowHealthColor && (playerHealth <= lowHealthValue))
        {
            sliderThatIsColored.color = Color.red;
            updatedLowHealthColor = true;
        }

    }
}
