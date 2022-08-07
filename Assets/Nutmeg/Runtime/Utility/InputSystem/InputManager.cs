using UnityEngine;
using UnityEngine.InputSystem;

namespace Nutmeg.Runtime.Utility.InputSystem
{
    public class InputManager : MonoBehaviour
    {
        [SerializeField] private InputEventChannel eventChannel;

        public static InputActions input { get; private set; }

        private void Start()
        {
        }

        private void OnEnable()
        {
            input = new InputActions();

            
            //TODO use static input
            input.Player.Move.performed += MoveOnPerformed;
            input.Player.Move.canceled += MoveOnCanceled;
            input.Player.Primary.performed += PrimaryOnPerformed;
            input.Player.Primary.canceled += PrimaryOnCanceled;
            input.Player.Reload.performed += ReloadOnPerformed;
            input.Player.Reload.canceled += ReloadOnCanceled;
            input.Player.Secondary.performed += SecondaryOnPerformed;
            input.Player.Secondary.canceled += SecondaryOnCanceled;
            input.Player.Scroll.performed += ScrollOnPerformed;
            input.Player.Enable();
        }

        private void ScrollOnPerformed(InputAction.CallbackContext context)
        {
            eventChannel.PerformScrollAction(context);
        }

        private void SecondaryOnPerformed(InputAction.CallbackContext context)
        {
            eventChannel.PerformSecondaryAction(context);
        }

        private void SecondaryOnCanceled(InputAction.CallbackContext context)
        {
            eventChannel.CancelSecondaryAction(context);
        }

        private void ReloadOnPerformed(InputAction.CallbackContext context)
        {
            eventChannel.PerformReloadAction(context);
        }

        private void ReloadOnCanceled(InputAction.CallbackContext context)
        {
            eventChannel.CancelReloadAction(context);
        }

        private void PrimaryOnPerformed(InputAction.CallbackContext context)
        {
            eventChannel.PerformPrimaryAction(context);
        }

        private void PrimaryOnCanceled(InputAction.CallbackContext context)
        {
            eventChannel.CancelPrimaryAction(context);
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