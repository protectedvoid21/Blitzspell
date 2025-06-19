using Health.HealthScripts;
using UnityEngine;

namespace Entities.Attacks
{
    public class Projectile : MonoBehaviour
    {
        [Header("Projectile Settings")]
        [SerializeField] private float speed = 15.0f; // Prędkość kuli ognia
        [SerializeField] private float lifetime = 5.0f; // Czas życia kuli ognia przed autodestrukcją

        [Header("Collision Settings")]
        [SerializeField] private LayerMask hitLayer; // Warstwy, z którymi kula ognia ma kolidować (Ustaw w Inspector!)
        [SerializeField] private float raycastLookAheadDistance = 0.2f; // Dodatkowy dystans dla raycasta, aby wykryć obiekty tuż przed sobą

        [Header("Damage Settings")]
        [SerializeField] private float damageAmount = 20.0f; // Ilość zadawanych obrażeń

        [Header("Effects")]
        [SerializeField] private GameObject impactEffectPrefab; // Prefabrykat efektu po trafieniu (opcjonalne)

        private Vector3 direction; // Kierunek lotu kuli ognia
        private float timeSpawned; // Czas stworzenia pocisku, do obliczania czasu życia

        private void Update()
        {
            // Sprawdź czas życia pocisku
            // if (Time.time - timeSpawned > lifetime)
            // {
            //     Destroy(gameObject);
            //     return; // Zakończ Update, pocisk zostanie zniszczony
            // }

            // Oblicz dystans, jaki kula ognia pokona w tej klatce
            var moveDistance = speed * Time.deltaTime;

            // Wykonaj Raycast z aktualnej pozycji w kierunku ruchu
            // Physics.Raycast(początek, kierunek, out informacje_o_trafieniu, max_dystans, maska_warstw)
            // Dodajemy raycastLookAheadDistance do moveDistance, aby lepiej wykrywać kolizje *przed* dotarciem do obiektu.
            var raycastDistance = moveDistance + raycastLookAheadDistance;


            // Raycast zaczyna się od aktualnej pozycji, kieruje w 'direction', na dystans 'raycastDistance'.
            // Sprawdzamy tylko warstwy z 'hitLayer'.
            if (Physics.Raycast(transform.position, direction, out var hit, raycastDistance, hitLayer,
                    QueryTriggerInteraction.Collide))
            {
                // Trafiliśmy w coś!
                Debug.Log(
                    $"Fireball trafił w: {hit.collider.name} na warstwie: {LayerMask.LayerToName(hit.collider.gameObject.layer)}");

                // Przesuń kulę ognia do punktu trafienia przed zniszczeniem/obsłużeniem kolizji
                // hit.distance to dystans od początku raycasta do punktu trafienia.
                // Przesuwamy na ten dystans minus mały offset, żeby nie utknąć w obiekcie.
                // Można po prostu ustawić pozycję na hit.point, ale to może powodować zniekształcenia wizualne.
                // Lepsze jest przesunięcie o odległość do trafienia:
                transform.position += direction * hit.distance; // Przesuń do punktu trafienia

                // --- OBSŁUGA TRAFIENIA ---
                HandleHit(hit); // Wywołaj metodę obsługi trafienia
                // --------------------------------------

                // Zniszcz kulę ognia po trafieniu
                Destroy(gameObject);
            }
            else
            {
                // Nie trafiliśmy w nic w tej klatce, przesuwamy kulę ognia
                transform.position += direction * moveDistance;
            }
        }

        // Opcjonalnie: Wizualizacja Raycasta w edytorze (pomocne przy debugowaniu)
        private void OnDrawGizmosSelected() // Użyj OnDrawGizmosSelected, aby rysować tylko gdy obiekt jest zaznaczony
        {
            // Rysuj raycast w kierunku ruchu od aktualnej pozycji
            Gizmos.color = Color.red;
            var drawDistance = speed * Time.deltaTime + raycastLookAheadDistance;
            Gizmos.DrawRay(transform.position, direction * drawDistance);

            // Możesz też narysować sferę na końcu, aby lepiej wizualizować punkt kontrolny Raycasta
            // Gizmos.color = Color.yellow;
            // Gizmos.DrawWireSphere(transform.position + direction * drawDistance, 0.1f); // Rysuje sferę na końcu raycasta
        }

        // Metoda wywoływana przez skrypt rzucający czar, aby ustawić kierunek
        public void SetDirection(Vector3 newDirection)
        {
            if (newDirection == Vector3.zero)
            {
                Debug.LogWarning("Fireball direction set to zero. Destroying.");
                Destroy(gameObject); // Zniszcz pocisk, jeśli kierunek jest zerowy
                return;
            }

            direction = newDirection.normalized; // Normalizujemy, żeby prędkość była stała niezależnie od wektora
            transform.forward = direction; // Opcjonalnie: obróć obiekt w kierunku lotu

            // timeSpawned = Time.time; // Zapamiętaj czas stworzenia

            // Autodestrukcja po określonym czasie życia
            // Można to zrobić tutaj z Destroy(gameObject, lifetime);
            // lub w Update sprawdzając Time.time - timeSpawned > lifetime
            Destroy(gameObject, lifetime);
        }

        // Metoda do obsługi tego, co dzieje się po trafieniu (PLACEHOLDER)
        private void HandleHit(RaycastHit hit)
        {
            Debug.Log(
                $"Projectile trafił w: {hit.collider.name} na warstwie: {LayerMask.LayerToName(hit.collider.gameObject.layer)}");

            // --- NOWA LOGIKA BLOKOWANIA PRZEZ TARCZĘ ---
            // Sprawdź, czy trafiony obiekt (lub jego rodzic) ma tag "Shield".
            // Często tag ustawia się na głównym obiekcie tarczy, więc sprawdzamy bezpośrednio trafiony collider.
            if (hit.collider.CompareTag("Shield"))
            {
                Debug.Log("Fireball zablokowany przez tarczę!");

                // Opcjonalnie: Utwórz efekt trafienia w tarczę
                // if (shieldHitEffectPrefab != null)
                // {
                //     Instantiate(shieldHitEffectPrefab, hit.point, Quaternion.LookRotation(hit.normal));
                // }
                // Opcjonalnie: Odtwórz dźwięk odbicia/blokowania

                // Zniszcz pocisk, ponieważ został zablokowany.
                Destroy(gameObject);

                // WAŻNE: Zakończ metodę, aby nie aplikować obrażeń i nie tworzyć standardowego efektu trafienia
                return;
            }
            // --- KONIEC LOGIKI BLOKOWANIA ---

            // --- LOGIKA ZADAWANIA OBRAŻEŃ ---
            // Spróbuj znaleźć komponent implementujący IDamageable na trafionym obiekcie
            // lub jego rodzicach (często collider jest na dziecku, a HP na rodzicu)
            var damageableObject = hit.collider.GetComponentInParent<IDamageable>();

            if (damageableObject != null)
                // Jeśli znaleziono obiekt, który może otrzymać obrażenia, zadaj mu obrażenia
                damageableObject.TakeDamage(damageAmount);
            else
                // Opcjonalnie: Obsłuż trafienie w obiekty niezniszczalne i niebędące tarczami (np. ściany, podłoga)
                Debug.Log(
                    $"Hit object {hit.collider.name} is not Damageable and not a Shield. It's likely environment.");
            // Możesz dodać tutaj inny, specyficzny efekt trafienia w środowisko
            // --------------------------------------


            // --- LOGIKA TWORZENIA EFEKTÓW PO TRAFIENIU ---
            // Sprawdź, czy prefabrykat efektu trafienia jest ustawiony w Inspektorze
            if (impactEffectPrefab)
                // Utwórz efekt w miejscu trafienia, obrócony w kierunku normalnej powierzchni (od ściany)
                // Quaternion.LookRotation(hit.normal) tworzy rotację skierowaną "na zewnątrz" od powierzchni, w którą trafiłeś.
                Instantiate(impactEffectPrefab, hit.point, Quaternion.LookRotation(hit.normal));
            // ---------------------------------------------

            // Możesz dodać inne logiki, np. odgrywanie dźwięku trafienia.
        }
    }
}