using UnityEngine;
using UnityEngine.InputSystem;

namespace Nutmeg.Runtime.Utility.InputSystem
{
    public class InputManager : MonoBehaviour
    {
        [SerializeField] private InputEventChannel eventChannel;

        private PlayerInputActions input;

        private void Start()
        {
        }

        private void OnEnable()
        {
            input = new PlayerInputActions();

            input.Player.Move.performed += MoveOnPerformed;
            input.Player.Move.canceled += MoveOnCanceled;
            input.Player.Fire.performed += FireOnPerformed;
            input.Player.Fire.canceled += FireOnCanceled;
            input.Player.Reload.performed += ReloadOnPerformed;
            input.Player.Reload.canceled += ReloadOnCanceled;
            input.Player.Throw.performed += ThrowOnPerformed;
            input.Player.Throw.canceled += ThrowOnCanceled;
            input.Player.Scroll.performed += ScrollOnPerformed;
            input.Player.Enable();
        }

        private void ScrollOnPerformed(InputAction.CallbackContext context)
        {
            eventChannel.PerformScrollAction(context);
        }

        private void ThrowOnPerformed(InputAction.CallbackContext context)
        {
            eventChannel.PerformThrowAction(context);
        }

        private void ThrowOnCanceled(InputAction.CallbackContext context)
        {
            eventChannel.CancelThrowAction(context);
        }

        private void ReloadOnPerformed(InputAction.CallbackContext context)
        {
            eventChannel.PerformReloadAction(context);
        }

        private void ReloadOnCanceled(InputAction.CallbackContext context)
        {
            eventChannel.CancelReloadAction(context);
        }

        private void FireOnPerformed(InputAction.CallbackContext context)
        {
            eventChannel.PerformFireAction(context);
        }

        private void FireOnCanceled(InputAction.CallbackContext context)
        {
            eventChannel.CancelFireAction(context);
        }

        private void MoveOnPerformed(InputAction.CallbackContext context)
        {
            eventChannel.PerformMoveAction(context);
        }

        private void MoveOnCanceled(InputAction.CallbackContext context)
        {
            eventChannel.CancelMoveAction(context);
        }
    }
}