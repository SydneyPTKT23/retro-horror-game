namespace SLC.RetroHorror.Core
{
    public interface IInteractable
    {
        bool IsInteractable { get; }
        string InteractionMessage { get; }
        void OnInteract();
    }
}