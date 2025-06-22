using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Entities.Interactable
{
    public class Portal : MonoBehaviour, IInteractable
    {
        [Header("Interaction Prompt")]
        [SerializeField] private GameObject prompt;
        
        private TextMeshPro promptTextComponent;
        private bool isActive;

        private void Awake() // Użyj Awake, aby pobrać komponent na wczesnym etapie
        {
            if (prompt == null) return;

            promptTextComponent = prompt.GetComponent<TextMeshPro>();
            if (promptTextComponent == null)
            {
                Debug.LogError("TextMeshProUGUI component not found on the 'prompt' GameObject!", this);
            }
        }
        
        private void Start()
        {
            if (promptTextComponent != null)
            {
                promptTextComponent.text = "You need an opening rune first!";
            }

            if (prompt != null)
            {
                prompt.SetActive(false); 
            }
            isActive = false;
        }

        public void Interact()
        {
            if (isActive)
            {
                GameManager.OnPortalInteract();
            }
        }

        public void ShowPrompt()
        {
            if (prompt)
            {
                prompt.SetActive(true);
            }
        }

        public void HidePrompt()
        {
            if (prompt)
            {
                prompt.SetActive(false);
            }
        }

        public void RotatePrompt(Camera playerCamera)
        {
            if (prompt)
            {
                // Upewnij się, że tekst zawsze jest zwrócony w stronę kamery gracza
                prompt.transform.rotation = Quaternion.LookRotation(prompt.transform.position - playerCamera.transform.position);
            }
        }

        public void Activate()
        {
            Debug.Log("Activated");
            if (promptTextComponent != null)
            {
                promptTextComponent.text = "Press 'E' to enter the portal";
            }
            isActive = true;
        }
    }
}
