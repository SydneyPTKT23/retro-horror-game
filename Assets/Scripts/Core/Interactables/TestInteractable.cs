namespace SLC.RetroHorror.Core
{
    public class TestInteractable : InteractableBase
    {
        public override void OnInteract()
        {
            base.OnInteract();

            Destroy(gameObject);
        }
    }
}