// Interfejs do oznaczania obiektów, które mogą otrzymać obrażenia

namespace Health.HealthScripts
{
    public interface IDamageable
    {
        // Metoda, którą muszą zaimplementować wszystkie klasy implementujące ten interfejs
        void TakeDamage(float amount);
    }
}