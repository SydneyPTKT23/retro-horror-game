using SLC.RetroHorror.DataPersistence;
using UnityEngine;

namespace SLC.RetroHorror.Core
{
    public class InteractableBase : SaveableMonoBehaviour, IInteractable
    {
        [Space, Header("Interaction Settings")]
        [SerializeField] private bool isInteractable = true;
        [Tooltip("Show this message whenever within interaction range.")]
        [SerializeField] private string interactionMessage = "Interact";
        [SerializeField] private GameObject interactIndicator;

        #region Properties
        public bool IsInteractable => isInteractable;
        public string InteractionMessage => interactionMessage;
        public Collider InteractCollider { get; private set; }

        #endregion

        protected virtual void Start()
        {
            if (!TryGetComponent<Collider>(out Collider attachedCollider))
            {
                Debug.LogWarning($"Interactable {gameObject.name} does not have the collider required for interaction!");
            }
            InteractCollider = attachedCollider;
        }

        public virtual void OnInteract(InteractionController controller)
        {
            Debug.Log("Interacted with: " + gameObject.name);
        }

        public void ActivateIndicator()
        {
            if (interactIndicator != null) interactIndicator.SetActive(true);
        }

        public void DeactivateIndicator()
        {
            if (interactIndicator != null) interactIndicator.SetActive(false);
        }
    }
}