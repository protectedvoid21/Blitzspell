using UnityEngine;

namespace Entities.Attacks
{
    public abstract class BaseSpellCaster : MonoBehaviour
    {
        [Header("Prefabs")]
        [SerializeField] protected GameObject primarySpellPrefab;

        [SerializeField] protected GameObject secondarySpellPrefab;

        [Header("Spawn Point")]
        [SerializeField] protected Transform spawnPoint;

        [Header("Cooldowns")]
        [SerializeField] protected float primarySpellCooldown = 2.0f;
        [SerializeField] protected float secondarySpellCooldown = 1.0f;

        protected float lastPrimarySpellCastTime = -Mathf.Infinity;
        protected float lastSecondarySpellCastTime = -Mathf.Infinity;

        // Metoda do rzucania zaklęcia. Implementacja będzie specyficzna dla gracza/AI.
        protected abstract void CastPrimarySpell();
        protected abstract void CastSecondarySpell();

        // Metoda abstrakcyjna do pobierania kierunku celowania
        protected abstract Vector3 GetTargetDirection();

        protected void PerformSpellCast(GameObject spellPrefab, ref float lastCastTime, float cooldown)
        {
            if (!spellPrefab || !spawnPoint)
            {
                Debug.LogError($"Spell Prefab or Spawn Point not set on {gameObject.name}");
                return;
            }

            if (Time.time < lastCastTime + cooldown)
                // Cooldown jeszcze trwa
                return;

            var castDirection = GetTargetDirection();

            var spellGo = Instantiate(spellPrefab, spawnPoint.position, Quaternion.identity);
            var projectileScript = spellGo.GetComponent<Projectile>();

            if (projectileScript)
                projectileScript.SetDirection(castDirection);
            else
                Debug.LogError($"Prefab assigned to {spellPrefab.name} does not have a Projectile script.");

            lastCastTime = Time.time;
        }
    }
}