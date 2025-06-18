using System.Globalization;
using TMPro;
using UnityEngine;

namespace Player.HUD
{
    public class PlayerHUD : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI currentHealthText;
        [SerializeField] private TextMeshProUGUI maxHealthText;

        public void UpdateHealth(float currentHealth, float maxHealth)
        {
            currentHealthText.text = currentHealth.ToString(CultureInfo.InvariantCulture);
            maxHealthText.text = maxHealth.ToString(CultureInfo.InvariantCulture);
        }
    }
}