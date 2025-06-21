using UnityEngine;
using Entities.Interactable;

namespace Player.Controls
{
    public class PlayerInteraction : MonoBehaviour
    {
        [Header("Interaction Settings")]
        public float interactionDistance = 3f;
        public LayerMask interactableLayer;

        [Header("Camera")]
        [SerializeField] private Camera mainCamera;
        
        private IInteractable currentInteractable;
        
        private void Update()
        {
            // Sprawdzaj co klatkę, czy jest obiekt do podświetlenia
            CheckForInteractable();
        }
        
        private void CheckForInteractable()
        {
            IInteractable newInteractable = null;
            
            if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out var hit, interactionDistance, interactableLayer))
            {
                // Jeśli trafiliśmy w obiekt na warstwie "interactableLayer"
                newInteractable = hit.collider.GetComponentInParent<IInteractable>();
            }
            
            // Tu wywołaj funkcje wyświetlenie i usunięcia tekstu
            if (newInteractable != null && newInteractable != currentInteractable)
            {
                currentInteractable?.HidePrompt();
                newInteractable.ShowPrompt();
                newInteractable.RotatePrompt(mainCamera);
            }
            else if (newInteractable != null && newInteractable == currentInteractable)
            {
                currentInteractable.ShowPrompt();
                currentInteractable.RotatePrompt(mainCamera);
            }
            else if (newInteractable == null && currentInteractable != null)
            {
                currentInteractable.HidePrompt();
            }
            
            currentInteractable = newInteractable;
        }
        
        public void OnInteractPerformed()
        {
            if (currentInteractable != null)
            {
                currentInteractable.Interact();
                currentInteractable = null;
            }
            else
            {
                Debug.Log("Nic do interakcji w zasięgu.");
            }
        }
        
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(mainCamera.transform.position, mainCamera.transform.forward * interactionDistance);
        }
    }
}
