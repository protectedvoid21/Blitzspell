using UnityEngine;

namespace Entities.Enemy
{
    [RequireComponent(typeof(EnemySpellCaster))]
    public class RangedEnemyAI : EnemyAI
    {
        [Header("Ranged Attack Settings")] [SerializeField]
        private float attackRange = 15f; // Zasięg, w którym przeciwnik może atakować gracza

        private EnemySpellCaster enemySpellCaster;

        protected override void Awake()
        {
            base.Awake(); // Wywołaj Awake z klasy bazowej
            enemySpellCaster = GetComponent<EnemySpellCaster>();
        }

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos(); // Wywołaj Gizmos z klasy bazowej
            // Wizualizacja zasięgu ataku dystansowego
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }

        protected override void ChaseBehavior()
        {
            base.ChaseBehavior(); // Wywołaj ChaseBehavior z klasy bazowej

            // Dodatkowa logika dla RangeEnemyAI:
            if (playerTarget)
            {
                // Jeśli gracz jest w zasięgu ataku, przejdź do stanu Atakowania
                if (Vector3.Distance(transform.position, playerTarget.position) <= attackRange)
                {
                    currentState = EnemyState.Attacking;
                    agent.isStopped = true;
                }
                else
                {
                    agent.isStopped = false; // Upewnij się, że agent się porusza
                }
            }
        }

        protected override void AttackBehavior()
        {
            if (!playerTarget)
            {
                currentState = EnemyState.ReturningToPatrol;
                return;
            }

            // Obróć się w stronę gracza
            var lookDirection = playerTarget.position - transform.position;
            lookDirection.y = 0;
            if (lookDirection != Vector3.zero)
            {
                var targetRotation = Quaternion.LookRotation(lookDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
            }

            // Atakuj gracza za pomocą SpellCaster
            enemySpellCaster.SetTarget(playerTarget); // Ustaw cel dla SpellCaster
            enemySpellCaster.AttackTarget();

            // Jeśli gracz oddali się poza zasięg ataku, wróć do pościgu
            if (Vector3.Distance(transform.position, playerTarget.position) > attackRange + 1f)
            {
                currentState = EnemyState.Chasing;
                agent.isStopped = false;
            }
        }
    }
}