// Ten plik powinien nazywać się Health.cs
using UnityEngine;

// Implementujemy interfejs IDamageable, co oznacza, że musimy zaimplementować metodę TakeDamage
public class Health : MonoBehaviour, IDamageable
{
    [Header("Health Settings")]
    [SerializeField] protected HealthData maxHealth; // Maksymalne HP
    protected float currentHealth; // Bieżące HP

    // Opcjonalnie: Zdarzenie wywoływane po śmierci (możesz go użyć w innych skryptach)
    // public delegate void OnDeath(GameObject diedObject);
    // public static event OnDeath OnEntityDied;


    private void Awake()
    {
        // Inicjalizacja HP przy starcie
        currentHealth = maxHealth.GetMaxHealth();
        Debug.Log($"{gameObject.name} initialized with {currentHealth}/{maxHealth.GetMaxHealth()} HP.");
    }

    // Metoda z interfejsu IDamageable - wymagana do zaimplementowania
    public virtual void TakeDamage(float amount)
    {
        if (amount <= 0)
        {
            Debug.LogWarning("Damage amount should be positive.");
            return;
        }

        // Zmniejsz HP
        currentHealth -= amount;
        Debug.Log($"{gameObject.name} received {amount} damage. Current HP: {currentHealth}/{maxHealth.GetMaxHealth()}");

        // Sprawdź, czy obiekt umarł
        if (!(currentHealth <= 0)) return;
        currentHealth = 0; // Upewnij się, że HP nie schodzi poniżej zera
        Die(); // Wywołaj metodę obsługi śmierci

        // Opcjonalnie: Wywołaj zdarzenie zmiany HP, jeśli masz UI paski życia itp.
        // if (OnHealthChanged != null) OnHealthChanged.Invoke(currentHealth, maxHealth);
    }

    // Opcjonalnie: Metoda leczenia
    public virtual void Heal(float amount)
    {
        if (amount <= 0)
        {
            Debug.LogWarning("Heal amount should be positive.");
            return;
        }

        currentHealth += amount;
        // Ogranicz HP do maksymalnego
        currentHealth = Mathf.Min(currentHealth, maxHealth.GetMaxHealth());
        Debug.Log($"{gameObject.name} healed {amount}. Current HP: {currentHealth}/{maxHealth.GetMaxHealth()}");

        // Opcjonalnie: Wywołaj zdarzenie zmiany HP
        // if (OnHealthChanged != null) OnHealthChanged.Invoke(currentHealth, maxHealth);
    }

    // Metoda obsługująca śmierć obiektu (PLACEHOLDER)
    private void Die()
    {
        Debug.Log($"{gameObject.name} has died.");

        // --- TUTAJ DODAJSZ LOGIKĘ ŚMIERCI DLA TEGO OBIEKTU ---
        // Przykłady:
        // - Wyłączenie skryptów ruchu/AI: GetComponent<PlayerMove>()?.enabled = false; GetComponent<EnemyAI>()?.enabled = false;
        // - Odtworzenie animacji śmierci: GetComponent<Animator>()?.SetTrigger("Die");
        // - Zniszczenie obiektu po pewnym czasie: Destroy(gameObject, 3f);
        // - Natychmiastowe zniszczenie: Destroy(gameObject);
        // - Ukrycie obiektu: gameObject.SetActive(false);
        // - Zrzucenie przedmiotów: GetComponent<LootDropper>()?.DropLoot();
        // -----------------------------------------------------

        // Opcjonalnie: Wywołaj globalne zdarzenie śmierci, jeśli inne systemy tego potrzebują
        // if (OnEntityDied != null) OnEntityDied.Invoke(gameObject);

        // Dla prostoty na początku możesz po prostu zniszczyć obiekt:
        Destroy(gameObject);
    }

    // Opcjonalnie: Metoda do sprawdzenia, czy obiekt żyje
    public bool IsAlive()
    {
        return currentHealth > 0;
    }

    // Opcjonalnie: Metoda do pobrania aktualnego HP
    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    // Opcjonalnie: Metoda do pobrania maksymalnego HP
    public float GetMaxHealth()
    {
        return maxHealth.GetMaxHealth();
    }
}
