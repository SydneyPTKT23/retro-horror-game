using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SLC.RetroHorror.Input
{
    [CreateAssetMenu (menuName = "InputReader")]
    public class InputReader : ScriptableObject, InputMap.IPlayerActions, InputMap.IUIActions, InputMap.IDialogueActions
    {
        private InputMap inputMap;
        public bool PlayerInputsEnabled => inputMap.Player.enabled;
        public bool DialogueInputsEnabled => inputMap.Dialogue.enabled;

        public event Action<Vector2> MoveEvent;
        public event Action InteractEvent;
        public event Action InteractEventCancelled;
        public event Action AttackEvent;
        public event Action AttackEventCancelled;
        public event Action SprintEvent;
        public event Action SprintEventCancelled;
        public event Action QuickturnEvent;
        public event Action QuickturnEventCancelled;

        //I'm ngl chief I don't remember if any of these will actually be used. These
        //are only here because they're part of the default input map :p
        //I'll remove them before any relevant builds if they're not staying
        public event Action CrouchEvent;
        public event Action CrouchEventCancelled;
        public event Action JumpEvent;
        public event Action JumpEventCancelled;

        //Dialogue input actions
        public event Action<Vector2> DialogueNavigateEvent;
        public event Action DialogueSubmitEvent;
        public event Action DialogueSubmitCancelledEvent;

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
                inputMap.Dialogue.SetCallbacks(this);
                inputMap.Player.Disable();
                inputMap.Dialogue.Disable();
            }
        }

        private void OnDisable()
        {
            inputMap.Player.Disable();
        }

        public void EnablePlayerInput()
        {
            inputMap.Player.Enable();
            inputMap.Dialogue.Disable();
        }

        public void DisablePlayerInput()
        {
            inputMap.Player.Disable();
        }

        public void EnableDialogueInput()
        {
            inputMap.Player.Disable();
            inputMap.Dialogue.Enable();
        }

        #region player input actions

        public void OnMove(InputAction.CallbackContext context)
        {
            MoveEvent?.Invoke(obj: context.ReadValue<Vector2>());
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

        public void OnQuickTurn(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Started)
            {
                QuickturnEvent?.Invoke();
            }
            else if (context.phase == InputActionPhase.Canceled)
            {
                QuickturnEventCancelled?.Invoke();
            }
        }

        public void OnDialogueNavigate(InputAction.CallbackContext context)
        {
            DialogueNavigateEvent?.Invoke(obj: context.ReadValue<Vector2>());
        }

        public void OnDialogueSubmit(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Started)
            {
                DialogueSubmitEvent?.Invoke();
            }
            else if (context.phase == InputActionPhase.Canceled)
            {
                DialogueSubmitCancelledEvent?.Invoke();
            }
        }

        #endregion
    }
}