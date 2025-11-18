using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUi : MonoBehaviour
{
    [SerializeField] Slider healthSlider;
    [SerializeField] TMP_Text healthText;

    public void UpdateHealthUi(int currentHealth, int maxHealth)
    {
        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;

        healthText.text = $"Health: {currentHealth} / {maxHealth}";
    }
}
