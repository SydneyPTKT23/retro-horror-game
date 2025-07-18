using System.Collections.Generic;
using System.Linq;
using SLC.RetroHorror.Input;
using UnityEngine;

namespace SLC.RetroHorror.Core
{
    public class InteractionController : MonoBehaviour
    {
        private Transform player;

        [Header("Input Variables")]
        [SerializeField] private InputReader inputReader;

        [Header("Interaction Settings")]
        [SerializeField] private BoxCollider interactionCollider;
        private List<InteractableBase> interactables;

        private void Start()
        {
            player = GetComponentInParent<PlayerController>().transform;
            if (player == null) Debug.LogError("InteractionController couldn't find MovementController!");

            interactables = new();
            inputReader.InteractEvent += HandleInteractDown;
            inputReader.InteractEventCancelled += HandleInteractUp;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other == null || !other.TryGetComponent<InteractableBase>(out var interactable)) return;

            if (!interactables.Contains(interactable))
            {
                interactables.Add(interactable);

                //Sort available colliders by distance to player, do interaction with closest interactable
                interactables.OrderBy(col => Vector3.Distance(player.position, col.transform.position));

                for (int i = 0; i < interactables.Count; i++)
                {
                    bool closestActivated = false;
                    if (!closestActivated)
                    {
                        interactables[i].ActivateIndicator();
                        closestActivated = true;
                    }
                    else interactables[i].DeactivateIndicator();
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other == null || !other.TryGetComponent<InteractableBase>(out var interactable)) return;

            if (interactables.Contains(interactable))
            {
                interactables.Remove(interactable);

                //Sort available colliders by distance to player, do interaction with closest interactable
                interactables.OrderBy(col => Vector3.Distance(player.position, col.transform.position));

                for (int i = 0; i < interactables.Count; i++)
                {
                    bool closestActivated = false;
                    if (!closestActivated)
                    {
                        interactables[i].ActivateIndicator();
                        closestActivated = true;
                    }
                    else interactables[i].DeactivateIndicator();
                }
            }
        }

        private void TryInteract()
        {
            if (interactables == null || interactables.Count == 0) return;
            interactables[0].OnInteract(this);
        }

        private void HandleInteractDown()
        {
            TryInteract();
        }

        private void HandleInteractUp()
        {

        }

        public void RemoveColliderFromInteractableList(InteractableBase _interactable)
        {
            if (_interactable == null) return;

            if (interactables.Contains(_interactable))
            {
                interactables.Remove(_interactable);
            }
        }

        private void OnDrawGizmos()
        {
            if (interactables == null) Gizmos.color = Color.green;
            else Gizmos.color = interactables.Count == 0 ? Color.green : Color.red;
            Gizmos.matrix = interactionCollider.transform.localToWorldMatrix;
            Gizmos.DrawWireCube(Vector3.zero, interactionCollider.size);
        }
    }
}