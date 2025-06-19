using UnityEngine;

namespace Menu.PauseMenu
{
    public class PauseScript : MonoBehaviour
    {
        private static PauseScript instance;
        private bool isGamePaused;
        
        private void Awake()
        {
            if(!instance)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public static void OnPause()
        {
            if (!instance) return;
            instance.isGamePaused = true;
            Time.timeScale = 0;
        }

        public static void OnResume()
        {
            if (!instance) return;
            instance.isGamePaused = false;
            Time.timeScale = 1;
        }

        public static bool GetIsGamePaused()
        {
            return instance && instance.isGamePaused;
        }
    }
}
