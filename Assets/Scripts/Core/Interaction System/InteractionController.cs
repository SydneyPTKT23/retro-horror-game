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
        private List<Collider> activeColliders;

        private void Start()
        {
            player = GetComponentInParent<MovementTankController>().transform;
            if (player == null) Debug.LogError("InteractionController couldn't find MovementController!");

            activeColliders = new();
            inputReader.InteractEvent += HandleInteractDown;
            inputReader.InteractEventCancelled += HandleInteractUp;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other == null) return;

            if (!activeColliders.Contains(other))
            {
                activeColliders.Add(other);

                //Sort available colliders by distance to player, do interaction with closest interactable
                activeColliders.OrderBy(col => Vector3.Distance(player.position, transform.position));

                for (int i = 0; i < activeColliders.Count; i++)
                {
                    bool closestActivated = false;
                    if (activeColliders[i].TryGetComponent<IInteractable>(out var interactable))
                    {
                        if (!closestActivated)
                        {
                            interactable.ActivateIndicator();
                            closestActivated = true;
                        }
                        else interactable.DeactivateIndicator();
                    }
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other == null) return;

            if (activeColliders.Contains(other))
            {
                activeColliders.Remove(other);

                //Sort available colliders by distance to player, do interaction with closest interactable
                activeColliders.OrderBy(col => Vector3.Distance(player.position, transform.position));

                for (int i = 0; i < activeColliders.Count; i++)
                {
                    bool closestActivated = false;
                    if (activeColliders[i].TryGetComponent<IInteractable>(out var interactable))
                    {
                        if (!closestActivated)
                        {
                            interactable.ActivateIndicator();
                            closestActivated = true;
                        }
                        else interactable.DeactivateIndicator();
                    }
                }
            }
        }

        private void TryInteract()
        {
            //Sort available colliders by distance to player, do interaction with closest interactable
            activeColliders.OrderBy(col => Vector3.Distance(player.position, transform.position));

            for (int i = 0; i < activeColliders.Count; i++)
            {
                if (activeColliders[i].TryGetComponent<IInteractable>(out var interactable))
                {
                    interactable.OnInteract(this);
                    break;
                }
            }
        }

        private void HandleInteractDown()
        {
            TryInteract();
        }

        private void HandleInteractUp()
        {

        }

        public void RemoveColliderFromInteractableList(Collider collider)
        {
            if (collider == null) return;

            if (activeColliders.Contains(collider))
            {
                activeColliders.Remove(collider);
            }
        }

        private void OnDrawGizmos()
        {
            if (activeColliders == null) Gizmos.color = Color.green;
            else Gizmos.color = activeColliders.Count == 0 ? Color.green : Color.red;
            Gizmos.matrix = interactionCollider.transform.localToWorldMatrix;
            Gizmos.DrawWireCube(Vector3.zero, interactionCollider.size);
        }
    }
}