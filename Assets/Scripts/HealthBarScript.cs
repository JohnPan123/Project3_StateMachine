using UnityEngine;
using UnityEngine.UI;

public class HealthBarScript : MonoBehaviour
{
    [SerializeField] private GameObject player;
    private PlayerScript playerScript;

    private Slider healthSlider;

    private const float playerMaxHeatlh = 100f;

    void Start()
    {
        playerScript = player.GetComponent<PlayerScript>();

        healthSlider = GetComponent<Slider>();
        healthSlider.value = playerMaxHeatlh;
    }
    void Update()
    {
        healthSlider.value = playerScript.GetPlayerHealth;
    }
}
