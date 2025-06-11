using System.Collections;
using SLC.RetroHorror.Input;
using UnityEngine;

namespace SLC.RetroHorror.Core
{
    [RequireComponent(typeof(CharacterController))]
    public class MovementTankController : MonoBehaviour
    {
        [Header("Input Variables")]
        [SerializeField] private InputReader inputReader;
        private bool shiftDown;
        [SerializeField] private float inputThreshold = 0.5f;

        [Header("Movement Settings")]
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
        private Health m_health;

        private RaycastHit m_hitInfo;

        [Header("Acceleration Settings")]
        [SerializeField] private float inputScaler = 5f;
        [SerializeField] private float movementScaler = 5f;

        [Space, Header("DEBUG")]
        [SerializeField] private Vector2 m_inputVector;
        [SerializeField] private Vector2 scaledMovementInput;
        [SerializeField] private float scaledMovementSpeed;
        [SerializeField] private Vector3 m_finalMoveVector;
        [Space]
        [SerializeField] private float m_currentSpeed;
        [Space]
        [SerializeField] private float m_finalRayLength;
        [SerializeField] private bool m_isGrounded;

        public float killHeight = -50.0f;
        public bool IsDead { get; private set; }

        #region Default Methods

        private void Start()
        {
            m_characterController = GetComponent<CharacterController>();

            m_health = GetComponent<Health>();
            m_health.OnDie += OnDie;

            m_finalRayLength = rayLength + m_characterController.center.y;
            m_isGrounded = true;

            //Subscribe to input methods
            inputReader.EnablePlayerInput();
            SubscribeInputEvents();
        }

        private void Update()
        {
            // Autokill player if they manage to fall out of the map to prevent softlocking.
            if (!IsDead && transform.position.y < killHeight)
            {
                m_health.Kill();
            }

            if (m_characterController)
            {
                CheckIfGrounded();

                HandleMovement();
                HandleRotation();

                CalculateMovementSpeed();
                ApplyGravity();
            } 
        }

        #endregion

        #region Input Methods

        private void SubscribeInputEvents()
        {
            inputReader.MoveEvent += HandleMoveVector;
            inputReader.SprintEvent += HandleShiftDown;
            inputReader.SprintEventCancelled += HandleShiftUp;
        }

        private void HandleMoveVector(Vector2 _moveVector)
        {
            m_inputVector = _moveVector;
        }

        private Vector2 GetScaledInput()
        {
            return scaledMovementInput = Vector2.Lerp(scaledMovementInput, m_inputVector, Time.deltaTime * inputScaler);
        }

        private float GetScaledSpeed()
        {
            return scaledMovementSpeed = Mathf.Lerp(scaledMovementSpeed, m_currentSpeed, Time.deltaTime * movementScaler);
        }

        private void HandleShiftDown()
        {
            shiftDown = true;
        }

        private void HandleShiftUp()
        {
            shiftDown = false;
        }

        #endregion

        private void OnDie()
        {
            IsDead = true;
        }

        private void CheckIfGrounded()
        {
            // Manually check for grounded because the CharacterController default is less reliable.
            Vector3 t_origin = transform.position + m_characterController.center;
            bool t_hitGround = Physics.SphereCast(t_origin, raySphereRadius, Vector3.down, out m_hitInfo, m_finalRayLength, groundLayer);

            // Draw the groundcheck for convenience.
            Debug.DrawRay(t_origin, Vector3.down * rayLength, Color.red);
            m_isGrounded = t_hitGround;
        }

        private bool CanRun()
        {
            return true;
        }

        private void HandleMovement()
        {
            Vector3 t_desiredDirection = GetScaledInput().y * transform.forward;
            Vector3 t_flatDirection = FlattenVectorOnSlopes(t_desiredDirection);

            Vector3 t_finalVector = GetScaledSpeed() * t_flatDirection;

            m_finalMoveVector.x = t_finalVector.x;
            m_finalMoveVector.z = t_finalVector.z;

            if (m_characterController.isGrounded)
                m_finalMoveVector.y += t_finalVector.y;

            m_characterController.Move(m_finalMoveVector * Time.deltaTime);
        }

        private Vector3 FlattenVectorOnSlopes(Vector3 t_flattenedVector)
        {
            // Correct movement on slopes to keep speed consistent.
            if (m_isGrounded)
                t_flattenedVector = Vector3.ProjectOnPlane(t_flattenedVector, m_hitInfo.normal);

            return t_flattenedVector;
        }

        private void HandleRotation()
        {
            float t_desiredRotation = m_inputVector.x * turnSpeed;
            transform.Rotate(0, t_desiredRotation * Time.deltaTime, 0);
        }

        private void CalculateMovementSpeed()
        {
            m_currentSpeed = shiftDown && CanRun() ? runSpeed : walkSpeed;
            m_currentSpeed = m_inputVector == Vector2.zero ? 0f : m_currentSpeed;
            m_currentSpeed = m_inputVector.y < -inputThreshold ? m_currentSpeed * moveBackwardModifier : m_currentSpeed;
        }

        private void ApplyGravity()
        {
            // If grounded, add a little bit of extra downward force just in case.
            if (m_characterController.isGrounded)
                m_finalMoveVector.y = -stickToGroundForce;

            m_finalMoveVector += gravityMultiplier * Time.deltaTime * Physics.gravity;
        }
    }
}