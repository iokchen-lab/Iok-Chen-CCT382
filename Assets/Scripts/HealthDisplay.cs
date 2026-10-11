using UnityEngine;
using TMPro;

public class HealthDisplay : MonoBehaviour
{
    [SerializeField] private GameEventSO healthData;
    [SerializeField] private TMP_Text healthText;

    private void Start()
    {
        UpdateHealthText();
    }

    public void UpdateHealthText()
    {
        if (healthData == null || healthText == null)
            return;

        healthText.text = $"Health: {healthData.currentHealth:0}";
    }
}