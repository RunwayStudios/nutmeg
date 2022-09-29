using UnityEngine;
using UnityEngine.InputSystem;

namespace Nutmeg.Runtime.Utility.InputSystem
{
    public class InputManager : MonoBehaviour
    {
        [SerializeField] private InputEventChannel eventChannel;

        public static InputActions Input { get; private set; }

        private void Start()
        {
        }

        private void OnEnable()
        {
            Input = new InputActions();

            
            //TODO use static input
            Input.Player.Move.performed += MoveOnPerformed;
            Input.Player.Move.canceled += MoveOnCanceled;
            Input.Player.Primary.performed += PrimaryOnPerformed;
            Input.Player.Primary.canceled += PrimaryOnCanceled;
            Input.Player.Reload.performed += ReloadOnPerformed;
            Input.Player.Reload.canceled += ReloadOnCanceled;
            Input.Player.Secondary.performed += SecondaryOnPerformed;
            Input.Player.Secondary.canceled += SecondaryOnCanceled;
            Input.Player.Scroll.performed += ScrollOnPerformed;
            Input.Player.Enable();
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