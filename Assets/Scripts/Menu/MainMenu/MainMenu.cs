using Menu.PauseMenu;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Menu.MainMenu
{
    public class MainMenu : MonoBehaviour
    {
        public void PlayGame()
        {
            PauseScript.OnResume();
            SceneManager.LoadScene("Scenes/Level_1");
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}
