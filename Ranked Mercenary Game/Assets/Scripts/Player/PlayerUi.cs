using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUi : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] Slider healthSlider;
    [SerializeField] TMP_Text healthText;
    [Header("Mana")]
    [SerializeField] Slider manaSlider;
    [SerializeField] TMP_Text manaText;

    public void UpdateHealthUi(int currentHealth, int maxHealth)
    {
        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;

        healthText.text = $"Health: {currentHealth} / {maxHealth}";
    }

    public void UpdateManaUi(int currentMana, int maxMana)
    {
        manaSlider.maxValue = maxMana;
        manaSlider.value = currentMana;

        manaText.text = $"Mana: {currentMana} / {maxMana}";
    }
}
