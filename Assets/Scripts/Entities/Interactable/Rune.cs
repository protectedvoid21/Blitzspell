using Managers;
using UnityEngine;

namespace Entities.Interactable
{
    public class Rune : PickupItem
    {
        public override void Interact()
        {
            Debug.Log("Activate portal to next level");
            Level1GameManager.RunePickedUp();

            Destroy(gameObject); // Usuń obiekt po podniesieniu
        }
    }
}
