using Player.HUD;
using UnityEngine;

namespace Player.Health
{
    public class PlayerHealth : global::Health.HealthScripts.Health
    {
        [SerializeField] private PlayerHUD hud;

        private void Start()
        {
            hud.UpdateHealth(currentHealth, maxHealth.GetMaxHealth());
        }

        public override void TakeDamage(float amount)
        {
            base.TakeDamage(amount);
            hud.UpdateHealth(currentHealth, maxHealth.GetMaxHealth());
        }
    }
}