using Menu.PauseMenu;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class Level3GameManager : GameManager
    {
        protected override void PortalInteract()
        {
            base.PortalInteract();
            PauseScript.OnPause();
            SceneManager.LoadScene("Scenes/EndScene");
        }
    }
}
