using UnityEngine;

namespace Entities.Interactable
{
    public class Rune : PickupItem
    {
        public override void Interact()
        {
            Debug.Log("Activate portal to next level");

            Destroy(gameObject); // Usuń obiekt po podniesieniu
        }
    }
}
