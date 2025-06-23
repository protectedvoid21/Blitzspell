using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class EndSceneManager : MonoBehaviour
    {
        public void GoToMenu()
        {
            SceneManager.LoadScene("Scenes/Menu");
        }
    }
}
