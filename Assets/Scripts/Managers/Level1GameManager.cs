using Entities.Interactable;
using UnityEngine;

namespace Managers
{
    public class Level1GameManager : MonoBehaviour
    {
        [Header("Enemies")]
        [SerializeField] private GameObject[] _enemies;

        [Header("Portals")]
        [SerializeField] private GameObject[] _portals;
        [SerializeField] private GameObject _bigPortal;

        [Header("Rune")]
        [SerializeField] private GameObject _rune;
        
        private static Level1GameManager instance;
        private static GameObject[] enemies;
        private static GameObject[] portals;
        private static GameObject bigPortal;
        private static GameObject rune;
        
        private static int enemiesCount;
        private static int portalsCount;

        private void Awake()
        {
            if (!instance)
            {
                instance = this;
                enemies = _enemies;
                portals = _portals;
                bigPortal = _bigPortal;
                rune = _rune;
                enemiesCount = enemies.Length;
                portalsCount = portals.Length;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public static void RunePickedUp()
        {
            Debug.Log("Picked up rune");
            bigPortal.GetComponent<Portal>()?.Activate();
        }

        public static void TargetDied(Vector3 point, string name)
        {
            // Sprawdzanie, czy tablice zostały prawidłowo zainicjowane
            if (enemies == null || portals == null)
            {
                Debug.LogError("Enemies or Portals array is not initialized!");
                return;
            }
            
            // Sprawdzanie, czy runa została prawidłowo zainicjowana
            if (!rune)
            {
                Debug.LogError("Rune is not initialized!");
                return;
            }

            if (name.Contains("Enemy")) enemiesCount--;
            if (name.Contains("Portal")) portalsCount--;
            
            if (enemiesCount != 0 || portalsCount != 0) return;
            
            Debug.Log(point);
            rune.transform.position = name.Contains("Portal") ? point + Vector3.up * 1f : point;
            rune.SetActive(true);
            Debug.Log("All targets destroyed! Rune activated.");
        }
    }
}
