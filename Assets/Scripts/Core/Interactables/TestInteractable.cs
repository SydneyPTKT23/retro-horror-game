using UnityEngine;

namespace SLC.RetroHorror.Core
{
    public class TestInteractable : InteractableBase
    {
        public override void OnInteract(InteractionController controller)
        {
            base.OnInteract(controller);

            Destroy(gameObject);
            controller.RemoveColliderFromInteractableList(gameObject.GetComponent<Collider>());
        }
    }
}