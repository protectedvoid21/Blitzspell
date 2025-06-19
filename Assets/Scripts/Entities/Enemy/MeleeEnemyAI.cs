using UnityEngine;
using Health.HealthScripts;

namespace Entities.Enemy
{
    public class MeleeEnemyAI : EnemyAI
    {
        [Header("Melee Attack Settings")]
        [SerializeField] private float attackRange = 2f; // Zasięg, w którym przeciwnik może zaatakować wręcz
        [SerializeField] private float attackCooldown = 1.5f; // Czas między atakami
        [SerializeField] private float attackDamage = 10f; // Obrażenia zadawane przez atak wręcz
        [SerializeField] private Transform attackPoint; // Punkt, z którego sprawdzamy trafienie ataku
        [SerializeField] private float attackRadius = 0.5f; // Promień detekcji trafienia ataku

        private float lastAttackTime = -Mathf.Infinity;

        protected override void ChaseBehavior()
        {
            base.ChaseBehavior(); // Wywołaj ChaseBehavior z klasy bazowej

            // Dodatkowa logika dla MeleeEnemyAI:
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
            
            // Ustaw stoppingDistance specyficzny dla ataku wręcz
            agent.stoppingDistance = attackRange * 0.8f;

            // Sprawdź cooldown ataku
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                PerformMeleeAttack();
                lastAttackTime = Time.time;
            }

            // Jeśli gracz oddali się poza zasięg ataku, wróć do pościgu
            if (Vector3.Distance(transform.position, playerTarget.position) > attackRange + 0.5f)
            {
                currentState = EnemyState.Chasing;
                agent.isStopped = false;
            }
        }

        private void PerformMeleeAttack()
        {
            if (!attackPoint)
            {
                Debug.LogWarning("Attack Point nie jest ustawiony dla " + gameObject.name);
                return;
            }

            var hitObjects = Physics.OverlapSphere(attackPoint.position, attackRadius, playerLayer);

            foreach (var hitCol in hitObjects)
            {
                var damageable = hitCol.GetComponent<IDamageable>();
                if (damageable == null) continue;
                damageable.TakeDamage(attackDamage);
                Debug.Log($"{gameObject.name} zaatakował {hitCol.name} i zadał {attackDamage} obrażeń.");
            }
        }

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos(); // Wywołaj Gizmos z klasy bazowej
            // Wizualizacja zasięgu ataku wręcz
            Gizmos.color = Color.magenta;
            if (attackPoint != null)
            {
                Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
            }
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
    }
}