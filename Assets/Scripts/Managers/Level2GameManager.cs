using UnityEngine.SceneManagement;

namespace Managers
{
    public class Level2GameManager : GameManager
    {
        protected override void PortalInteract()
        {
            base.PortalInteract();
            SceneManager.LoadScene("Scenes/Menu");
        }
    }
}
