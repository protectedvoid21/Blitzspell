using Managers;
using UnityEngine;

namespace Entities.Enemy
{
    public class EnemyHealth : Health.HealthScripts.Health
    {
        protected override void Die()
        {
            Debug.Log($"{gameObject.name} has died.");
            
            Level1GameManager.TargetDied(transform.position, gameObject.name);
            
            Destroy(gameObject);
        }
    }
}
