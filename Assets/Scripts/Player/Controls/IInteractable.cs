using UnityEngine;

namespace Player.Controls
{
    public interface IInteractable
    {
        void Interact();
        void ShowPrompt();
        void HidePrompt();
        void RotatePrompt(Camera playerCamera);
    }
}
