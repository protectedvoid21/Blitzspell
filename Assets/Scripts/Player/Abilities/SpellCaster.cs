using Entities.Attacks;
using Player.HUD;
using Menu.PauseMenu;
using UnityEngine;

namespace Player.Abilities
{
    public class SpellCaster : BaseSpellCaster
    {
        [Header("Player Specific Settings")]
        [SerializeField] private Transform playerCameraTransform;

        [SerializeField] private float aimDistance = 100f; // Dystans do punktu celowania od kamery

        [Header("Crosshair")]
        [SerializeField] private CrosshairController crosshair;
        [SerializeField] private float crosshairScaleDuration = 0.1f;

        private bool castPrimaryInputReceived;
        private bool castSecondaryInputReceived;

        private void Update()
        {
            if (castPrimaryInputReceived)
            {
                CastPrimarySpell();
                castPrimaryInputReceived = false;
            }
            else if (castSecondaryInputReceived)
            {
                CastSecondarySpell();
                castSecondaryInputReceived = false;
            }
        }

        protected override void CastPrimarySpell()
        {
            // Przypisanie prefabów w Inspectorze będzie teraz "Primary Spell Prefab" zamiast "Fireball Prefab"
            PerformSpellCast(primarySpellPrefab, ref lastPrimarySpellCastTime, primarySpellCooldown);
            if (crosshair) crosshair.SetScale(CrosshairScale.Cast, crosshairScaleDuration);
        }

        protected override void CastSecondarySpell()
        {
            // Przypisanie prefabów w Inspectorze będzie teraz "Secondary Spell Prefab" zamiast "Lightning Prefab"
            PerformSpellCast(secondarySpellPrefab, ref lastSecondarySpellCastTime, secondarySpellCooldown);
            if (crosshair) crosshair.SetScale(CrosshairScale.Cast, crosshairScaleDuration);
        }

        protected override Vector3 GetTargetDirection()
        {
            if (!playerCameraTransform)
            {
                Debug.LogError("Player Camera Transform not set on " + gameObject.name);
                return Vector3.forward; // Zwróć domyślny kierunek, aby uniknąć błędu
            }

            Vector3 targetPoint;
            var ray = new Ray(playerCameraTransform.position, playerCameraTransform.forward);

            if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity,
                    LayerMask.GetMask("Entity", "Environment", "Ground", "Default", "Enemy")))
                targetPoint = hitInfo.point;
            else
                targetPoint = playerCameraTransform.position + playerCameraTransform.forward * aimDistance;
            return (targetPoint - spawnPoint.position).normalized;
        }

        // Metoda wywoływana przez InputManager
        public void OnPrimarySpellCast()
        {
            if (PauseScript.GetIsGamePaused()) return;
            if (Time.time < lastPrimarySpellCastTime + primarySpellCooldown)
            {
                Debug.Log("Primary spell is on cooldown.");
                return;
            }

            castPrimaryInputReceived = true;
        }

        public void OnSecondarySpellCast()
        {
            if (PauseScript.GetIsGamePaused()) return;
            if (Time.time < lastSecondarySpellCastTime + secondarySpellCooldown)
            {
                Debug.Log("Secondary spell is on cooldown.");
                return;
            }

            castSecondaryInputReceived = true;
        }
    }
}