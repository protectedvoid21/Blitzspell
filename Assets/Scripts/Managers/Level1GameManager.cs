using Entities.Interactable;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class Level1GameManager : GameManager
    {
        protected override void PortalInteract()
        {
            base.PortalInteract();
            SceneManager.LoadScene("Scenes/Level_2");
        }
    }
}
