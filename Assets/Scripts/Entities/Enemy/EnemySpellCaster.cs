using Entities.Attacks;
using UnityEngine;

namespace Entities.Enemy
{
    public class EnemySpellCaster : BaseSpellCaster
    {
        [Header("Enemy Spell Settings")]
        [SerializeField] private GameObject spellPrefabToUse; // Prefab zaklęcia, które przeciwnik będzie rzucał

        [SerializeField] private float spellCooldown = 2.0f;
        [SerializeField] private float attackRange = 15f; // Zasięg, w którym przeciwnik może atakować gracza

        private Transform target; // Cel

        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }

        public void AttackTarget()
        {
            if (!target)
            {
                Debug.LogWarning($"{gameObject.name}: No target set for attack.");
                return;
            }

            // Sprawdź, czy cel jest w zasięgu ataku
            if (Vector3.Distance(transform.position, target.position) > attackRange) return; // Cel poza zasięgiem ataku

            // Przeciwnik rzuca tylko jedno zaklęcie, więc używamy primarySpellPrefab
            if (Time.time < lastPrimarySpellCastTime + spellCooldown)
                // Cooldown jeszcze trwa
                return;

            // Celuj w target i rzuć zaklęcie
            PerformSpellCast(spellPrefabToUse, ref lastPrimarySpellCastTime, spellCooldown);
        }

        protected override void CastPrimarySpell()
        {
            // Ta metoda nie będzie używana przez przeciwnika, ale musi być zaimplementowana.
        }

        protected override void CastSecondarySpell()
        {
            // Ta metoda nie będzie używana przez przeciwnika, ale musi być zaimplementowana.
        }

        protected override Vector3 GetTargetDirection()
        {
            return !target
                ?
                // Jeśli nie ma celu, rzuć w domyślnym kierunku
                transform.forward
                : (target.position - spawnPoint.position).normalized;
        }
    }
}