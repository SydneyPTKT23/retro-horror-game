using UnityEngine;

namespace SLC.RetroHorror.Core
{
    public class InteractionController : MonoBehaviour
    {
        [Space, Header("Interaction Settings")]
        [SerializeField] private Transform m_interactionCollider;
        [SerializeField] private bool m_isInteracting;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                CheckForInteractables();
            }
        }

        private void CheckForInteractables()
        {
            if (m_isInteracting) return;

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

        public void OnInteractionStart()
        {
            // Time.timeScale = 0.0f;
            m_isInteracting = true;
        }

        public void OnInteractionEnd()
        {
            // Time.timeScale = 1.0f;
            m_isInteracting = false;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.matrix = m_interactionCollider.localToWorldMatrix;
            Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
        }
    }
}