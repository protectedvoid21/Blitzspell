using System.Collections;
using UnityEngine;
using Menu.PauseMenu;

namespace Player.Abilities
{
    public class PlayerShield : MonoBehaviour
    {
        [Header("Shield Settings")]
        [SerializeField] private GameObject shield;
        [SerializeField] private float shieldDeployTime = 0.5f;
        [SerializeField] private float shieldCooldown = 2.0f;

        private bool isShieldActive;
        private float lastShieldDeactivatedTime = -Mathf.Infinity;

        private void DeployShield()
        {
            // Use shield
            if (shield)
                shield.SetActive(true);

            StartCoroutine(DeactivateShieldAfterDelay(shieldDeployTime));
        }

        private IEnumerator DeactivateShieldAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            if (shield)
                shield.SetActive(false);

            // Ustaw flagę na false
            isShieldActive = false;
            Debug.Log("Shield deactivated.");

            // Zapisz aktualny czas, aby wiedzieć, od kiedy liczyć cooldown
            lastShieldDeactivatedTime = Time.time;
            Debug.Log(
                $"Shield cooldown started. Ready again at Time.time = {lastShieldDeactivatedTime + shieldCooldown}");
        }

        public void OnShieldActive()
        {
            if (PauseScript.GetIsGamePaused()) return;
            // --- COOLDOWN ---
            // Sprawdź, czy minął wystarczający czas od ostatniej dezaktywacji tarczy
            if (Time.time < lastShieldDeactivatedTime + shieldCooldown)
            {
                // Cooldown jeszcze trwa, nie można użyć tarczy.
                Debug.Log("Shield is on cooldown.");
                return; // Zakończ metodę, nie aktywuj tarczy
            }

            // Jeśli nie ma cooldownu i tarcza nie jest już aktywna, aktywuj ją.
            if (!isShieldActive)
            {
                // Ustaw flagę na true
                isShieldActive = true;
                Debug.Log("Shield activated.");

                // Wywołaj logikę rozmieszczenia tarczy i jej dezaktywacji po czasie
                DeployShield();
            }
            else
            {
                Debug.Log("Shield is already active.");
            }
        }
    }
}