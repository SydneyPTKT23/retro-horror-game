using UnityEngine;

namespace SLC.RetroHorror.Core
{
    public class InteractableBase : MonoBehaviour, IInteractable
    {
        [Space, Header("Interaction Settings")]
        [SerializeField] private bool isInteractable = true;
        [Tooltip("Show this message whenever within interaction range.")]
        [SerializeField] private string interactionMessage = "Interact";

        #region Properties
        public bool IsInteractable => isInteractable;
        public string InteractionMessage => interactionMessage;
        #endregion

        public virtual void OnInteract()
        {
            Debug.Log("Interacted with: " + gameObject.name);
        }
    }
}