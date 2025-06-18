using UnityEngine;

namespace Health.HealthScripts
{
    [CreateAssetMenu(fileName = "HealthData", menuName = "Scriptable Objects/HealthData")]
    public class HealthData : ScriptableObject
    {
        [SerializeField] private float maxHealth;

        public float GetMaxHealth()
        {
            return maxHealth;
        }
    }
}