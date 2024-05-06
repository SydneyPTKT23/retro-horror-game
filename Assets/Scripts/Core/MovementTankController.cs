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

        private CharacterController m_characterController;
        private InputHandler m_inputHandler;
        private Health m_health;

        private RaycastHit m_hitInfo;

        [Space, Header("DEBUG")]
        [SerializeField] private Vector2 m_inputVector;

        [SerializeField] private Vector3 m_finalMoveDirection;
        [Space]
        [SerializeField] private Vector3 m_finalMoveVector;

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

            m_inputHandler = GetComponent<InputHandler>();

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
            m_inputVector = m_inputHandler.InputVector;

            Vector3 t_desiredDirection = m_inputVector.y * transform.forward;
            Vector3 t_flatDirection = FlattenVectorOnSlopes(t_desiredDirection);

            m_finalMoveDirection = t_flatDirection;

            Vector3 t_finalVector = m_currentSpeed * m_finalMoveDirection;

            m_finalMoveVector.x = t_finalVector.x;
            m_finalMoveVector.z = t_finalVector.z;

            if (m_characterController.isGrounded)
                m_finalMoveVector.y += t_finalVector.y;


            float t_desiredRotation = m_inputVector.x * turnSpeed;
            transform.Rotate(0, t_desiredRotation * Time.deltaTime, 0);
        }

        private Vector3 FlattenVectorOnSlopes(Vector3 t_flattenedVector)
        {
            if (m_isGrounded)
            {
                t_flattenedVector = Vector3.ProjectOnPlane(t_flattenedVector, m_hitInfo.normal);
            }

            return t_flattenedVector;
        }

        private void CalculateMovementSpeed()
        {
            m_currentSpeed = Input.GetKey(KeyCode.LeftShift) && CanRun() ? runSpeed : walkSpeed;
            m_currentSpeed = !m_inputHandler.InputDetected ? 0.0f : walkSpeed;
            m_currentSpeed = m_inputHandler.InputVector.y == -1 ? m_currentSpeed * moveBackwardModifier : m_currentSpeed;
        }

        private void AddDownForce()
        {
            // If grounded, add 
            if (m_characterController.isGrounded)
                m_finalMoveVector.y = -stickToGroundForce;

            m_finalMoveVector += gravityMultiplier * Time.deltaTime * Physics.gravity;
        }

        private void AddMovement()
        {
            m_characterController.Move(m_finalMoveVector * Time.deltaTime);
        }
    }
}