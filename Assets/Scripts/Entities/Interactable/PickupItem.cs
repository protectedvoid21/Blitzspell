using Entities.Interactable;
using UnityEngine;

namespace Entities.Interactable
{
    public class PickupItem : MonoBehaviour, IInteractable
    {
        public string itemName = "Niezdefiniowany przedmiot";
        
        [Header("Interaction Prompt")]
        [SerializeField] private GameObject prompt; // Prefab tekstu 3D

        public virtual void Interact()
        {
            Debug.Log("Podniesiono: " + itemName + "!");

            Destroy(gameObject); // Usuń obiekt po podniesieniu
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
                prompt.transform.rotation = Quaternion.LookRotation(prompt.transform.position - playerCamera.transform.position);
            }
        }
    }
}
