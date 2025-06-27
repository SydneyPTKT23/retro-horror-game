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
        [SerializeField] private float exhaustedMaxSpeedPenaltyMult = 0.6f;
        [SerializeField] private float turnSpeed = 180.0f;
        [SerializeField] private float moveBackwardModifier = 0.5f;
        [SerializeField] private float quickturnTime = 0.5f;
        private WaitForSeconds quickturnWait;
        [SerializeField] private float quickturnCooldown = 0.1f;
        private WaitForSeconds quickturnCooldownWait;
        [SerializeField] private float maxStamina = 100f;
        [SerializeField] private float staminaExhaustionBuffer = 20f;
        [SerializeField] private float staminaDrainPerSecond = 10f;
        [SerializeField] private float staminaRegenDelay = 5f;      //seconds
        [SerializeField] private float staminaRegenExhaustedMult = 1.5f;    //stamina regen delay is multiplied by this if exhausted
        private float staminaRegenTimer;
        [SerializeField] private float staminaRegenPerSecond = 5f;
        [SerializeField] private float staminaRegenStandingMultiplier = 1.5f;

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
        [SerializeField] private float currentSpeedPenaltyMult = 1f;
        [Space]
        [SerializeField] private float finalRayLength;
        [SerializeField] private bool isGrounded;

        /// <summary>
        /// Did you mean to use CurrentStamina?
        /// </summary>
        [SerializeField] private float currentStamina;
        private float CurrentStamina
        {
            get
            {
                return currentStamina;
            }

            set
            {
                if (value < currentStamina && currentStamina < maxStamina)
                {
                    StaminaDrained();
                }
                currentStamina = value;
            }
        }

        [Space, Header("Misc")]
        public float killHeight = -50.0f;
        public bool IsDead { get; private set; }

        //Helper variables not visible in editor
        private bool IsMoving => currentSpeed > 0f;
        private bool isExhausted = false;
        private bool disableMovement = false;       //movement disabling is handled directly in relevant methods
        private bool quickturnOnCooldown = false;
        private bool staminaRegenActive = false;
        private Coroutine staminaDrainCoroutine;
        private Coroutine staminaExhaustedDrainCoroutine;
        private Coroutine staminaRegenCoroutine;
        private LTDescr speedPenaltyTween;

        #region Default Methods

        private void Start()
        {
            characterController = GetComponent<CharacterController>();

            health = GetComponent<Health>();
            health.OnDie += OnDie;

            //Initialize variables
            finalRayLength = rayLength + characterController.center.y;
            isGrounded = true;
            CurrentStamina = maxStamina;
            quickturnWait = new(quickturnTime);
            quickturnCooldownWait = new(quickturnCooldown);

            //Subscribe to input methods
            inputReader.EnablePlayerInput();
            SubscribeInputEvents();
        }

        private void Update()
        {
            //Autokill player if they manage to fall out of the map to prevent softlocking.
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
            inputReader.QuickturnEvent += HandleQuickturn;
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
            staminaDrainCoroutine = StartCoroutine(DrainStamina());
        }

        private void HandleShiftUp()
        {
            shiftDown = false;
            if (staminaDrainCoroutine != null) StopCoroutine(staminaDrainCoroutine);
            if (staminaExhaustedDrainCoroutine != null) StopCoroutine(staminaExhaustedDrainCoroutine);
            if (speedPenaltyTween != null)
            {
                LeanTween.cancel(speedPenaltyTween.id);
                speedPenaltyTween = null;
            }
        }

        private void HandleQuickturn()
        {
            if (!quickturnOnCooldown) StartCoroutine(QuickturnLerp());
        }

        private IEnumerator QuickturnLerp()
        {
            quickturnOnCooldown = true;
            disableMovement = true;

            //Calc new rotation
            //By default turns clockwise, but if player is turning counterclockwise, respects that
            float newRotation = inputVector.x < 0 ? 180f : -180f;
            newRotation += transform.rotation.eulerAngles.y;

            //Start rotation and wait for it to finish
            LeanTween.rotateY(gameObject, newRotation, quickturnTime).setEaseInOutCubic();
            yield return quickturnWait;

            yield return quickturnCooldownWait;
            quickturnOnCooldown = false;
            disableMovement = false;
        }

        #endregion

        #region Controller Methods

        private void OnDie()
        {
            IsDead = true;
            //Cancel all active tweens & coroutines affecting the player gameObject
            LeanTween.cancelAll(gameObject);
            StopAllCoroutines();
        }

        private void CheckIfGrounded()
        {
            //Manually check for grounded because the CharacterController default is less reliable.
            Vector3 origin = transform.position + characterController.center;
            bool hitGround = Physics.SphereCast(origin, raySphereRadius, Vector3.down, out hitInfo, finalRayLength, groundLayer);

            //Draw the groundcheck for convenience.
            Debug.DrawRay(origin, Vector3.down * rayLength, Color.red);
            isGrounded = hitGround;
        }

        private bool CanRun()
        {
            return !isExhausted;
        }

        private void HandleMovement()
        {
            if (disableMovement) return;

            Vector3 desiredDirection = GetScaledInput().y * transform.forward;
            Vector3 flatDirection = FlattenVectorOnSlopes(desiredDirection);

            Vector3 finalVector = GetScaledSpeed() * flatDirection;

            finalMoveVector.x = finalVector.x;
            finalMoveVector.z = finalVector.z;

            if (characterController.isGrounded)
                finalMoveVector.y += finalVector.y;

            characterController.Move(finalMoveVector * Time.deltaTime);
        }

        private Vector3 FlattenVectorOnSlopes(Vector3 _flattenedVector)
        {
            //Correct movement on slopes to keep speed consistent.
            if (isGrounded)
                _flattenedVector = Vector3.ProjectOnPlane(_flattenedVector, hitInfo.normal);

            return _flattenedVector;
        }

        private void HandleRotation()
        {
            if (disableMovement) return;

            float t_desiredRotation = inputVector.x * turnSpeed;
            transform.Rotate(0, t_desiredRotation * Time.deltaTime, 0);
        }

        private void CalculateMovementSpeed()
        {
            currentSpeed = shiftDown && CanRun() ? runSpeed : walkSpeed;
            currentSpeed *= currentSpeedPenaltyMult;
            currentSpeed = inputVector.y < 0.05f ? 0f : currentSpeed;
            currentSpeed = inputVector.y < -inputThreshold ? currentSpeed * moveBackwardModifier : currentSpeed;
        }

        private void ApplyGravity()
        {
            //If grounded, add a little bit of extra downward force just in case.
            if (characterController.isGrounded)
                finalMoveVector.y = -stickToGroundForce;

            finalMoveVector += gravityMultiplier * Time.deltaTime * Physics.gravity;
        }

        //This is ran whenever stamina is set to an amount less than it was before
        //with the exception that if currentStamina was above maxStamina, it's not ran
        private void StaminaDrained()
        {
            if (staminaRegenActive)
            {
                staminaRegenTimer = 0f;
                StopCoroutine(staminaRegenCoroutine);
                staminaRegenCoroutine = StartCoroutine(RegenerateStamina());
            }
            else if (staminaRegenCoroutine == null)
            {
                staminaRegenTimer = 0f;
                staminaRegenCoroutine = StartCoroutine(RegenerateStamina());
            }
            else staminaRegenTimer = 0;
        }

        //This is the coroutine that drains stamina, started when shift is pressed
        //and ended when shift is released
        private IEnumerator DrainStamina()
        {
            float drainAmount;              //drain gaaaaaang (declaring here so we don't end
                                            //up with a billion declarations in the loop)
            while (CurrentStamina > 0f)
            {
                //If not moving, keep looping without draining stamina
                if (!IsMoving)
                {
                    yield return null;
                    continue;
                }

                drainAmount = staminaDrainPerSecond * Time.deltaTime;
                CurrentStamina -= drainAmount;
                yield return null;
            }

            staminaExhaustedDrainCoroutine = StartCoroutine(ExhaustionDrain());
        }

        private IEnumerator ExhaustionDrain()
        {
            float scaledTweenTime = (CurrentStamina + staminaExhaustionBuffer) / staminaDrainPerSecond;
            speedPenaltyTween = LeanTween.value(currentSpeedPenaltyMult, exhaustedMaxSpeedPenaltyMult, scaledTweenTime).
                setOnUpdate((v) => currentSpeedPenaltyMult = v).setEaseInQuad();

            float timeToComplete = 0;
            while (CurrentStamina > -staminaExhaustionBuffer)
            {
                CurrentStamina -= staminaDrainPerSecond * Time.deltaTime;
                timeToComplete += Time.deltaTime;
                yield return null;
            }

            speedPenaltyTween = null;
            isExhausted = true;
        }

        private IEnumerator RegenerateStamina()
        {
            float realRegenDelay = isExhausted ? staminaRegenDelay * staminaRegenExhaustedMult : staminaRegenDelay;
            //Delay before stamina starts regenerating, done with a while loop to more
            //easily restart delay if it's still active and stamina is drained again
            while (staminaRegenTimer < realRegenDelay)
            {
                if (isExhausted && IsMoving && shiftDown) staminaRegenTimer = 0;
                else staminaRegenTimer += Time.deltaTime;
                yield return null;
            }

            staminaRegenActive = true;
            float realStaminaRegen;

            while (CurrentStamina < maxStamina)
            {
                //Regen stamina: if moving, regen normal amount, if still, regen increased amount
                realStaminaRegen = IsMoving ? staminaRegenPerSecond * Time.deltaTime : staminaRegenPerSecond * staminaRegenStandingMultiplier * Time.deltaTime;
                CurrentStamina += realStaminaRegen;

                if (isExhausted && CurrentStamina > 25f)
                {
                    isExhausted = false;
                    currentSpeedPenaltyMult = 1;
                }

                yield return null;
            }

            //Coroutine is finishing up, reset some variables
            staminaRegenActive = false;
            if (CurrentStamina > maxStamina) CurrentStamina = maxStamina;
            staminaRegenCoroutine = null;
        }
        
        #endregion
    }
}