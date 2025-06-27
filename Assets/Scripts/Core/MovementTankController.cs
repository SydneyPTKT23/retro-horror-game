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

        private CharacterController characterController;
        private Health health;

        private RaycastHit hitInfo;

        [Header("Acceleration Settings")]
        [SerializeField] private float inputScaler = 5f;
        [SerializeField] private float movementScaler = 5f;

        [Space, Header("DEBUG")]
        [SerializeField] private Vector2 inputVector;
        [SerializeField] private Vector2 scaledMovementInput;
        [SerializeField] private float scaledMovementSpeed;
        [SerializeField] private Vector3 finalMoveVector;
        [Space]
        [SerializeField] private float currentSpeed;
        [Space]
        [SerializeField] private float finalRayLength;
        [SerializeField] private bool isGrounded;

        public float killHeight = -50.0f;
        public bool IsDead { get; private set; }

        #region Default Methods

        private void Start()
        {
            characterController = GetComponent<CharacterController>();

            health = GetComponent<Health>();
            health.OnDie += OnDie;

            finalRayLength = rayLength + characterController.center.y;
            isGrounded = true;

            //Subscribe to input methods
            inputReader.EnablePlayerInput();
            SubscribeInputEvents();
        }

        private void Update()
        {
            // Autokill player if they manage to fall out of the map to prevent softlocking.
            if (!IsDead && transform.position.y < killHeight)
            {
                health.Kill();
            }

            if (characterController)
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
            inputVector = _moveVector;
        }

        private Vector2 GetScaledInput()
        {
            return scaledMovementInput = Vector2.Lerp(scaledMovementInput, inputVector, Time.deltaTime * inputScaler);
        }

        private float GetScaledSpeed()
        {
            return scaledMovementSpeed = Mathf.Lerp(scaledMovementSpeed, currentSpeed, Time.deltaTime * movementScaler);
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
            Vector3 t_origin = transform.position + characterController.center;
            bool t_hitGround = Physics.SphereCast(t_origin, raySphereRadius, Vector3.down, out hitInfo, finalRayLength, groundLayer);

            // Draw the groundcheck for convenience.
            Debug.DrawRay(t_origin, Vector3.down * rayLength, Color.red);
            isGrounded = t_hitGround;
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

            finalMoveVector.x = t_finalVector.x;
            finalMoveVector.z = t_finalVector.z;

            if (characterController.isGrounded)
                finalMoveVector.y += t_finalVector.y;

            characterController.Move(finalMoveVector * Time.deltaTime);
        }

        private Vector3 FlattenVectorOnSlopes(Vector3 _flattenedVector)
        {
            // Correct movement on slopes to keep speed consistent.
            if (isGrounded)
                _flattenedVector = Vector3.ProjectOnPlane(_flattenedVector, hitInfo.normal);

            return _flattenedVector;
        }

        private void HandleRotation()
        {
            float t_desiredRotation = inputVector.x * turnSpeed;
            transform.Rotate(0, t_desiredRotation * Time.deltaTime, 0);
        }

        private void CalculateMovementSpeed()
        {
            currentSpeed = shiftDown && CanRun() ? runSpeed : walkSpeed;
            currentSpeed = inputVector == Vector2.zero ? 0f : currentSpeed;
            currentSpeed = inputVector.y < -inputThreshold ? currentSpeed * moveBackwardModifier : currentSpeed;
        }

        private void ApplyGravity()
        {
            // If grounded, add a little bit of extra downward force just in case.
            if (characterController.isGrounded)
                finalMoveVector.y = -stickToGroundForce;

            finalMoveVector += gravityMultiplier * Time.deltaTime * Physics.gravity;
        }
    }
}