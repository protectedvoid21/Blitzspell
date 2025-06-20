using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Entities.Enemy
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(EnemySpellCaster))]
    public class EnemyAI : MonoBehaviour
    {
        [Header("Patrol Settings")]
        [SerializeField] private Transform[] patrolPoints; // Punkty na ścieżce patrolowej
        [SerializeField] private float patrolSpeed = 2f;
        [SerializeField] private float patrolWaitTime = 1f; // Czas oczekiwania na punkcie patrolowym
        [SerializeField] private float arrivalThreshold = 0.6f;

        [Header("Detection Settings")]
        [SerializeField]
        protected LayerMask playerLayer; // Warstwa, na której znajduje się gracz

        [SerializeField] private float viewRadius = 10f; // Promień widzenia

        [Range(0, 360)] [SerializeField] private float viewAngle = 90f; // Kąt widzenia przeciwnika

        [SerializeField] private float losePlayerDistance = 20f; // Dystans, po którym przeciwnik wraca do patrolowania

        [Header("Chase Settings")]
        [SerializeField] private float chaseSpeed = 4f;
        [SerializeField] private float attackRange = 10f; // Zasięg, w którym przeciwnik zaczyna atakować

        protected NavMeshAgent agent;
        protected int currentPatrolPointIndex;

        protected EnemyState currentState;
        protected EnemySpellCaster enemySpellCaster;
        protected bool isPlayerDetected;
        protected Vector3 originalPatrolPathCenter; // Punkt odniesienia dla powrotu do patrolowania
        protected Quaternion originalPatrolRotation; // Rotacja odniesienia dla powrotu do patrolowania
        protected Transform playerTarget;
        protected bool returningToPatrolPath;
        protected bool isWaitingAtPatrolPoint;

        protected virtual void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            enemySpellCaster = GetComponent<EnemySpellCaster>();

            // Zapewnij, że NavMeshAgent jest włączony i skonfigurowany
            agent.speed = patrolSpeed;
            agent.stoppingDistance = 0.1f; // Mały dystans zatrzymania dla patrolu

            if (patrolPoints.Length > 0)
            {
                currentPatrolPointIndex = 0;
                SetNewPatrolDestination(); // Ustawiamy pierwszy cel
            }
            else
            {
                Debug.LogWarning("Brak punktów patrolowych dla: " + gameObject.name);
            }

            currentState = EnemyState.Patrolling;
        }

        private void Update()
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

        // Pomocnicza funkcja do wizualizacji stożka widzenia w edytorze
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
                {
                    Gizmos.DrawSphere(patrolPoints[i].position, 0.5f); // Rysuj małą sferę w punkcie
                    if (i < patrolPoints.Length - 1)
                        Gizmos.DrawLine(patrolPoints[i].position, patrolPoints[i + 1].position); // Połącz punkty
                    else if (patrolPoints.Length > 1) // Połącz ostatni z pierwszym, jeśli jest więcej niż jeden punkt
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
            // Sprawdź, czy gracz jest w zasięgu widzenia (promień i kąt)
            var targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, playerLayer);

            isPlayerDetected = false;
            foreach (var col in targetsInViewRadius)
            {
                var dirToTarget = (col.transform.position - transform.position).normalized;
                if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
                {
                    // Gracz w stożku widzenia
                    var distToTarget = Vector3.Distance(transform.position, col.transform.position);
                    if (!Physics.Raycast(transform.position, dirToTarget, out var hit, distToTarget))
                    {
                        // Nic nie zasłania gracza
                        playerTarget = col.transform;
                        isPlayerDetected = true;
                        break; // Gracz wykryty, wychodzimy z pętli
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

            // Zmiana stanu w zależności od wykrycia gracza
            if (isPlayerDetected)
            {
                if (currentState == EnemyState.Patrolling || currentState == EnemyState.ReturningToPatrol)
                {
                    currentState = EnemyState.Chasing;
                    agent.speed = chaseSpeed;
                    enemySpellCaster.SetTarget(playerTarget); // Ustaw cel dla SpellCaster
                }
            }
            else
            {
                // Gracz nie jest już widoczny, lub oddalił się za bardzo
                if (playerTarget && Vector3.Distance(transform.position, playerTarget.position) > losePlayerDistance)
                {
                    playerTarget = null; // Usuń cel
                    if (currentState is EnemyState.Chasing or EnemyState.Attacking)
                    {
                        currentState = EnemyState.ReturningToPatrol;
                        agent.speed = patrolSpeed;
                        agent.SetDestination(originalPatrolPathCenter); // Wróc do punktu, z którego odszedł
                    }
                }
            }
        }

        private void PatrolBehavior()
        {
            if (patrolPoints.Length == 0 || isWaitingAtPatrolPoint) return;

            // Sprawdź, czy przeciwnik dotarł do obecnego punktu patrolowego
            if (agent.pathPending == false && agent.remainingDistance <= arrivalThreshold)
            {
                StartCoroutine(WaitAtPatrolPoint());
            }
        }

        private IEnumerator WaitAtPatrolPoint()
        {
            isWaitingAtPatrolPoint = true; // Ustaw stan czekania
            
            // Zatrzymaj agenta
            agent.isStopped = true;
            
            if (patrolPoints[currentPatrolPointIndex])
                transform.rotation = patrolPoints[currentPatrolPointIndex].rotation;
            
            // Poczekaj na ustawiony czas
            yield return new WaitForSeconds(patrolWaitTime);
            
            // Wznów ruch i ustaw następny punkt patrolowy
            currentPatrolPointIndex = (currentPatrolPointIndex + 1) % patrolPoints.Length;
            SetNewPatrolDestination(); // Ustaw następny cel
            
            isWaitingAtPatrolPoint = false; // Zakończ stan czekania
            agent.isStopped = false; // Wznów ruch
        }

        protected virtual void ChaseBehavior()
        {
            if (!playerTarget)
            {
                currentState = EnemyState.ReturningToPatrol;
                return;
            }

            // Podążaj za graczem
            agent.SetDestination(playerTarget.position);

            // Sprawdź, czy gracz jest w zasięgu ataku
            if (Vector3.Distance(transform.position, playerTarget.position) <= attackRange)
            {
                currentState = EnemyState.Attacking;
                agent.isStopped = true; // Zatrzymaj się do ataku
            }
            else
            {
                agent.isStopped = false; // Upewnij się, że agent się porusza
            }
        }

        protected virtual void AttackBehavior()
        {
            if (!playerTarget)
            {
                currentState = EnemyState.ReturningToPatrol;
                return;
            }

            // Obróć się w stronę gracza
            var lookDirection = playerTarget.position - transform.position;
            lookDirection.y = 0; // Upewnij się, że obrót jest tylko w płaszczyźnie poziomej
            if (lookDirection != Vector3.zero)
            {
                var targetRotation = Quaternion.LookRotation(lookDirection);
                transform.rotation =
                    Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f); // Płynne obracanie
            }

            // Atakuj gracza
            enemySpellCaster.AttackTarget();

            // Jeśli gracz oddali się poza zasięg ataku, wróć do pościgu
            if (Vector3.Distance(transform.position, playerTarget.position) > attackRange + 1f) // Mały bufor
            {
                currentState = EnemyState.Chasing;
                agent.isStopped = false;
            }
        }

        private void ReturnToPatrolBehavior()
        {
            if (agent.pathPending == false && agent.remainingDistance <= arrivalThreshold)
            {
                // Dotarł do punktu odniesienia, wznów patrolowanie
                currentState = EnemyState.Patrolling;
                agent.isStopped = false; // Upewnij się, że agent jest w ruchu
                // Ustaw cel na aktualny punkt patrolowy, by kontynuować patrolowanie od miejsca, gdzie AI powinno być.
                SetNewPatrolDestination();
                transform.rotation = originalPatrolRotation; // Przywróć oryginalną rotację
            }
        }
        
        private void SetNewPatrolDestination()
        {
            if (patrolPoints.Length == 0) return;

            // Upewnij się, że cel jest poprawny, zanim go ustawisz
            if (patrolPoints[currentPatrolPointIndex])
            {
                agent.SetDestination(patrolPoints[currentPatrolPointIndex].position);
                // originalPatrolPathCenter powinien być punktem z którego AI odszedł,
                // a nie następnym punktem na ścieżce.
                // Logika dla originalPatrolPathCenter powinna być w momencie, gdy AI przechodzi ze stanu patrolowania na pogoń.
                // Pozostało bez zmian, ale warto mieć na uwadze.
                originalPatrolPathCenter = patrolPoints[currentPatrolPointIndex].position;
            }
            else
            {
                Debug.LogError($"Punkt patrolowy o indeksie {currentPatrolPointIndex} jest nullem!");
            }
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