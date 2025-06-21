using UnityEngine;

namespace Entities.Interactable
{
    public interface IInteractable
    {
        void Interact();
        void ShowPrompt();
        void HidePrompt();
        void RotatePrompt(Camera playerCamera);
    }
}
