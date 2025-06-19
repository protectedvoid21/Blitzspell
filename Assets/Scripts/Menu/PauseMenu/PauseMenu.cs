using UnityEngine;
using UnityEngine.SceneManagement;

namespace Menu.PauseMenu
{
    public class PauseMenu : MonoBehaviour
    {
        public GameObject pausePanel;

        public void Pause()
        {
            pausePanel.SetActive(true);
            PauseScript.OnPause();
            Time.timeScale = 0;
        }

        public void Resume()
        {
            pausePanel.SetActive(false);
            PauseScript.OnResume();
            Time.timeScale = 1;
        }

        public void Exit()
        {
            SceneManager.LoadScene("Scenes/Menu");
        }
    }
}
