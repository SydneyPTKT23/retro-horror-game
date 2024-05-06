using UnityEngine;

namespace SLC.RetroHorror.Core
{
    [RequireComponent(typeof(CharacterController))]
    public class MovementTankController : MonoBehaviour
    {
        [Space, Header("Movement Settings")]
        [SerializeField] private float walkSpeed = 3.0f;
        [SerializeField] private float runSpeed = 5.0f;
        [SerializeField] private float turnSpeed = 180.0f;
        [SerializeField] private float moveBackwardModifier = 0.5f;

        [Space, Header("Ground Settings")]
        [SerializeField] private float gravityMultiplier = 2.5f;
        [SerializeField] private float stickToGroundForce = 5.0f;
        [Space]
        [SerializeField] private LayerMask groundLayer = ~0;
        [SerializeField] private float rayLength = 0.1f;
        [SerializeField] private float raySphereRadius = 0.1f;

        private Health m_health;
        private CharacterController m_characterController;

        private RaycastHit m_hitInfo;

        [Space, Header("DEBUG")]
        [SerializeField] private Vector2 m_inputVector;
        [SerializeField] private Vector3 m_finalMovementVector;
        [Space]
        [SerializeField] private float m_currentSpeed;
        [Space]
        [SerializeField] private float m_finalRayLength;
        [SerializeField] private bool m_isGrounded;

        public float killHeight = -50.0f;
        public bool IsDead { get; private set; }

        private void Start()
        {
            m_characterController = GetComponent<CharacterController>();

            m_health = GetComponent<Health>();
            m_health.OnDie += OnDie;

            m_finalRayLength = rayLength + m_characterController.center.y;
            m_isGrounded = true;
        }

        private void Update()
        {
            if (!IsDead && transform.position.y < killHeight)
            {
                m_health.Kill();
            }

            if (m_characterController)
            {
                CheckIfGrounded();

                CalculateMovementDirection();
                CalculateMovementSpeed();

                AddDownForce();
                AddMovement();
            } 
        }

        private void OnDie()
        {
            IsDead = true;
        }

        private void CheckIfGrounded()
        {
            Vector3 t_origin = transform.position + m_characterController.center;
            bool t_hitGround = Physics.SphereCast(t_origin, raySphereRadius, Vector3.down, out m_hitInfo, m_finalRayLength, groundLayer);
            Debug.DrawRay(t_origin, Vector3.down * rayLength, Color.red);

            m_isGrounded = t_hitGround;
        }

        private bool CanRun()
        {
            return true;
        }

        private void CalculateMovementDirection()
        {
            m_inputVector.x = Input.GetAxisRaw("Horizontal");
            m_inputVector.y = Input.GetAxisRaw("Vertical");

            float x = m_inputVector.x * Time.deltaTime * turnSpeed;
            float y = m_inputVector.y * Time.deltaTime * m_currentSpeed;

            m_characterController.Move(transform.forward * y);
            m_characterController.transform.Rotate(0, x, 0);
        }

        private void CalculateMovementSpeed()
        {
            m_currentSpeed = Input.GetKey(KeyCode.LeftShift) && CanRun() ? runSpeed : walkSpeed;
            m_currentSpeed = m_inputVector.y == 0.0f ? 0.0f : walkSpeed;
            m_currentSpeed = m_inputVector.y == -1 ? m_currentSpeed * moveBackwardModifier : m_currentSpeed;
        }

        private void AddDownForce()
        {
            if (m_characterController.isGrounded && m_finalMovementVector.y < 0)
                m_finalMovementVector.y = -stickToGroundForce;

            m_finalMovementVector += gravityMultiplier * Time.deltaTime * Physics.gravity;
        }

        private void AddMovement()
        {
            m_characterController.Move(m_finalMovementVector * Time.deltaTime);
        }
    }
}