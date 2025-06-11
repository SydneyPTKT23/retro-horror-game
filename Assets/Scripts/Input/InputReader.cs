using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SLC.RetroHorror.Input
{
    [CreateAssetMenu (menuName = "InputReader")]
    public class InputReader : ScriptableObject, InputMap.IPlayerActions, InputMap.IUIActions
    {
        private InputMap inputMap;
        public bool PlayerInputsEnabled => inputMap.Player.enabled;

        public event Action<Vector2> MoveEvent;
        public event Action InteractEvent;
        public event Action InteractEventCancelled;

        //I'm ngl chief I don't remember if any of these will actually be used. These
        //are only here because they're part of the default input map :p
        public event Action AttackEvent;
        public event Action AttackEventCancelled;
        public event Action CrouchEvent;
        public event Action CrouchEventCancelled;
        public event Action JumpEvent;
        public event Action JumpEventCancelled;
        public event Action SprintEvent;
        public event Action SprintEventCancelled;

        //UI input actions in case we want to use those for something manually
        public event Action<Vector2> NavigateEvent;
        public event Action<Vector2> PointEvent;
        public event Action<Vector2> ScrollEvent;
        public event Action ClickEvent;
        public event Action ClickEventCancelled;
        public event Action MiddleClickEvent;
        public event Action MiddleClickEventCancelled;
        public event Action RightClickEvent;
        public event Action RightClickEventCancelled;
        public event Action CancelEvent;
        public event Action CancelEventCancelled;
        public event Action SubmitEvent;
        public event Action SubmitEventCancelled;
        public event Action NextEvent;
        public event Action NextEventCancelled;
        public event Action PreviousEvent;
        public event Action PreviousEventCancelled;

        private void OnEnable()
        {
            if (inputMap == null)
            {
                inputMap = new();
                inputMap.Player.SetCallbacks(this);
                inputMap.Player.Disable();
            }
        }

        private void OnDisable()
        {
            inputMap.Player.Disable();
        }

        public void EnablePlayerInput()
        {
            inputMap.Player.Enable();
        }

        public void DisablePlayerInput()
        {
            inputMap.Player.Disable();
        }

        #region player input actions

        public void OnMove(InputAction.CallbackContext context)
        {
            MoveEvent?.Invoke(obj: context.ReadValue<Vector2>());
            Debug.Log(context.ReadValue<Vector2>());
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Started)
            {
                InteractEvent?.Invoke();
            }
            else if (context.phase == InputActionPhase.Canceled)
            {
                InteractEventCancelled?.Invoke();
            }
        }

        public void OnAttack(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                AttackEvent?.Invoke();
            }
            else if (context.phase == InputActionPhase.Canceled)
            {
                AttackEventCancelled?.Invoke();
            }
        }

        public void OnCrouch(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                CrouchEvent?.Invoke();
            }
            else if (context.phase == InputActionPhase.Canceled)
            {
                CrouchEventCancelled?.Invoke();
            }
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                JumpEvent?.Invoke();
            }
            else if (context.phase == InputActionPhase.Canceled)
            {
                JumpEventCancelled?.Invoke();
            }
        }

        public void OnSprint(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                SprintEvent?.Invoke();
            }
            else if (context.phase == InputActionPhase.Canceled)
            {
                SprintEventCancelled?.Invoke();
            }
        }

        #endregion
        
        #region ui input actions

        public void OnNavigate(InputAction.CallbackContext context)
        {
            NavigateEvent?.Invoke(obj:context.ReadValue<Vector2>());
        }

        public void OnPoint(InputAction.CallbackContext context)
        {
            PointEvent?.Invoke(obj:context.ReadValue<Vector2>());
        }

        public void OnScrollWheel(InputAction.CallbackContext context)
        {
            ScrollEvent?.Invoke(obj:context.ReadValue<Vector2>());
        }

        public void OnClick(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                ClickEvent?.Invoke();
            }
            else if (context.phase == InputActionPhase.Canceled)
            {
                ClickEventCancelled?.Invoke();
            }
        }

        public void OnMiddleClick(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                MiddleClickEvent?.Invoke();
            }
            else if (context.phase == InputActionPhase.Canceled)
            {
                MiddleClickEventCancelled?.Invoke();
            }
        }

        public void OnRightClick(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                RightClickEvent?.Invoke();
            }
            else if (context.phase == InputActionPhase.Canceled)
            {
                RightClickEventCancelled?.Invoke();
            }
        }

        public void OnSubmit(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                SubmitEvent?.Invoke();
            }
            else if (context.phase == InputActionPhase.Canceled)
            {
                SubmitEventCancelled?.Invoke();
            }
        }

        public void OnCancel(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                CancelEvent?.Invoke();
            }
            else if (context.phase == InputActionPhase.Canceled)
            {
                CancelEventCancelled?.Invoke();
            }
        }

        public void OnNext(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                NextEvent?.Invoke();
            }
            else if (context.phase == InputActionPhase.Canceled)
            {
                NextEventCancelled?.Invoke();
            }
        }

        public void OnPrevious(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                PreviousEvent?.Invoke();
            }
            else if (context.phase == InputActionPhase.Canceled)
            {
                PreviousEventCancelled?.Invoke();
            }
        }

        #endregion
    }
}