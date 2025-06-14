using SLC.RetroHorror.Input;
using UnityEngine;

namespace SLC.RetroHorror.Core
{
    public class InteractionController : MonoBehaviour
    {
        [Header("Input Variables")]
        [SerializeField] private InputReader inputReader;
        private bool interactHeld = false;

        [Header("Interaction Settings")]
        [SerializeField] private Transform m_interactionCollider;

        private void Start()
        {
            inputReader.InteractEvent += HandleInteractDown;
            inputReader.InteractEventCancelled += HandleInteractUp;
        }

        private void CheckForInteractables()
        {
            // Collect valid interactions into an array of interactables.
            Collider[] t_collisions = Physics.OverlapBox(m_interactionCollider.position,
                m_interactionCollider.localScale * 0.5f, transform.rotation, ~0, QueryTriggerInteraction.Collide);

            for (int i = 0; i < t_collisions.Length; i++)
            {
                InteractableBase t_interactable = t_collisions[i].transform.GetComponent<InteractableBase>();

                if (t_interactable != null)
                {
                    t_interactable.OnInteract();
                }
            }
        }

        private void HandleInteractDown()
        {
            CheckForInteractables();
            interactHeld = true;
        }

        private void HandleInteractUp()
        {
            interactHeld = false;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.matrix = m_interactionCollider.localToWorldMatrix;
            Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
        }
    }
}