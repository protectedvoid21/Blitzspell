using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Entities.Enemy
{
    [RequireComponent(typeof(NavMeshAgent))]
    public abstract class EnemyAI : MonoBehaviour
    {
        [Header("Patrol Settings")] [SerializeField]
        protected Transform[] patrolPoints; // Punkty na ścieżce patrolowej

        [SerializeField] protected float patrolSpeed = 2f;
        [SerializeField] protected float patrolWaitTime = 1f; // Czas oczekiwania na punkcie patrolowym
        [SerializeField] protected float arrivalThreshold = 0.6f; // Próg do sprawdzenia dotarcia do punktu

        [Header("Detection Settings")] [SerializeField]
        protected LayerMask playerLayer; // Warstwa, na której znajduje się gracz

        [SerializeField] protected float viewRadius = 10f; // Promień widzenia

        [Range(0, 360)] [SerializeField] protected float viewAngle = 90f; // Kąt widzenia przeciwnika

        [SerializeField]
        protected float losePlayerDistance = 20f; // Dystans, po którym przeciwnik wraca do patrolowania

        [Header("Chase Settings")] [SerializeField]
        protected float chaseSpeed = 4f;

        protected NavMeshAgent agent;
        private int currentPatrolPointIndex;

        protected EnemyState currentState;
        private bool isPlayerDetected;
        private bool isWaitingAtPatrolPoint;
        private Vector3 originalPatrolPathCenter;
        private Quaternion originalPatrolRotation;
        protected Transform playerTarget;

        protected virtual void Awake()
        {
            agent = GetComponent<NavMeshAgent>();

            agent.speed = patrolSpeed;
            agent.stoppingDistance = 0.1f; // Mała wartość, bo używamy arrivalThreshold

            if (patrolPoints is { Length: > 0 })
            {
                currentPatrolPointIndex = 0;
                SetNewPatrolDestination();
            }
            else
            {
                Debug.LogWarning("Brak punktów patrolowych dla: " + gameObject.name);
            }

            currentState = EnemyState.Patrolling;
        }

        protected virtual void Update()
        {
            CheckForPlayer();

            switch (currentState)
            {
                case EnemyState.Patrolling:
                    PatrolBehavior();
                    break;
                case EnemyState.Chasing:
                    ChaseBehavior();
                    break;
                case EnemyState.Attacking:
                    AttackBehavior();
                    break;
                case EnemyState.ReturningToPatrol:
                    ReturnToPatrolBehavior();
                    break;
            }
        }

        // Pomocnicza funkcja do wizualizacji w edytorze
        protected virtual void OnDrawGizmos()
        {
            // Wizualizacja zasięgu widzenia
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, viewRadius);

            // Wizualizacja stożka widzenia
            var forward = transform.forward;
            var leftRayRotation = Quaternion.AngleAxis(-viewAngle / 2, Vector3.up);
            var rightRayRotation = Quaternion.AngleAxis(viewAngle / 2, Vector3.up);

            var leftRayDirection = leftRayRotation * forward;
            var rightRayDirection = rightRayRotation * forward;

            Gizmos.DrawRay(transform.position, leftRayDirection * viewRadius);
            Gizmos.DrawRay(transform.position, rightRayDirection * viewRadius);

            // Wizualizacja linii do gracza, jeśli wykryty
            if (isPlayerDetected && playerTarget != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, playerTarget.position);
            }

            // Wizualizacja punktów patrolowych
            if (patrolPoints is { Length: > 0 })
            {
                Gizmos.color = Color.blue;
                for (var i = 0; i < patrolPoints.Length; i++)
                    if (patrolPoints[i] != null)
                    {
                        Gizmos.DrawSphere(patrolPoints[i].position, 0.5f);
                        if (i < patrolPoints.Length - 1 && patrolPoints[i + 1] != null)
                            Gizmos.DrawLine(patrolPoints[i].position, patrolPoints[i + 1].position);
                        else if (patrolPoints.Length > 1 && patrolPoints[0] != null)
                            Gizmos.DrawLine(patrolPoints[i].position, patrolPoints[0].position);
                    }
            }

            // Wizualizacja miejsca powrotu
            if (currentState == EnemyState.ReturningToPatrol)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(originalPatrolPathCenter, 0.7f);
            }
        }

        private void CheckForPlayer()
        {
            var targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, playerLayer);

            isPlayerDetected = false;
            foreach (var col in targetsInViewRadius)
            {
                var dirToTarget = (col.transform.position - transform.position).normalized;
                if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
                {
                    var distToTarget = Vector3.Distance(transform.position, col.transform.position);
                    // Sprawdź, czy coś zasłania gracza, ignorując samego siebie
                    if (!Physics.Raycast(transform.position, dirToTarget, out var hit, distToTarget,
                            ~LayerMask.GetMask(LayerMask.LayerToName(gameObject.layer))))
                    {
                        // Nic nie zasłania gracza
                        playerTarget = col.transform;
                        isPlayerDetected = true;
                        break;
                    }

                    if (hit.collider.gameObject == col.gameObject)
                    {
                        // Raycast trafił w gracza, czyli nic go nie zasłania
                        playerTarget = col.transform;
                        isPlayerDetected = true;
                        break;
                    }
                }
            }

            if (isPlayerDetected)
            {
                if (currentState is EnemyState.Patrolling or EnemyState.ReturningToPatrol)
                {
                    currentState = EnemyState.Chasing;
                    agent.speed = chaseSpeed;
                }
            }
            else
            {
                if (playerTarget && Vector3.Distance(transform.position, playerTarget.position) > losePlayerDistance)
                {
                    playerTarget = null;
                    if (currentState is EnemyState.Chasing or EnemyState.Attacking)
                    {
                        currentState = EnemyState.ReturningToPatrol;
                        agent.speed = patrolSpeed;
                        agent.stoppingDistance = 0.1f; // Wróć do standardowego stoppingDistance
                        agent.SetDestination(originalPatrolPathCenter);
                    }
                }
            }
        }

        private void PatrolBehavior()
        {
            if (patrolPoints == null || patrolPoints.Length == 0 || isWaitingAtPatrolPoint) return;

            if (!agent.pathPending && agent.remainingDistance <= arrivalThreshold) StartCoroutine(WaitAtPatrolPoint());
        }

        private IEnumerator WaitAtPatrolPoint()
        {
            isWaitingAtPatrolPoint = true;
            agent.isStopped = true;

            yield return new WaitForSeconds(patrolWaitTime);

            currentPatrolPointIndex = (currentPatrolPointIndex + 1) % patrolPoints.Length;
            SetNewPatrolDestination();

            isWaitingAtPatrolPoint = false;
            agent.isStopped = false;
        }

        private void SetNewPatrolDestination()
        {
            if (patrolPoints == null || patrolPoints.Length == 0) return;

            if (patrolPoints[currentPatrolPointIndex])
            {
                agent.SetDestination(patrolPoints[currentPatrolPointIndex].position);
                originalPatrolPathCenter = patrolPoints[currentPatrolPointIndex].position;
                originalPatrolRotation = transform.rotation;
            }
            else
            {
                Debug.LogError($"Punkt patrolowy o indeksie {currentPatrolPointIndex} jest nullem!");
            }
        }

        protected virtual void ChaseBehavior()
        {
            if (!playerTarget)
            {
                currentState = EnemyState.ReturningToPatrol;
                return;
            }

            agent.SetDestination(playerTarget.position);

            // Sprawdź, czy gracz jest w zasięgu ataku
            if (Vector3.Distance(transform.position, playerTarget.position) <=
                agent.stoppingDistance + 0.1f) // Mały bufor
            {
                currentState = EnemyState.Attacking;
                agent.isStopped = true; // Zatrzymaj się do ataku
            }
            else
            {
                agent.isStopped = false; // Upewnij się, że agent się porusza
            }
        }

        protected abstract void AttackBehavior(); // Abstrakcyjna metoda ataku

        private void ReturnToPatrolBehavior()
        {
            if (agent.pathPending || !(agent.remainingDistance <= arrivalThreshold)) return;
            currentState = EnemyState.Patrolling;
            agent.isStopped = false;
            agent.stoppingDistance = 0.1f; // Resetuj stoppingDistance na patrol
            SetNewPatrolDestination();
            transform.rotation = originalPatrolRotation;
        }

        protected enum EnemyState
        {
            Patrolling,
            Chasing,
            Attacking,
            ReturningToPatrol
        }
    }
}