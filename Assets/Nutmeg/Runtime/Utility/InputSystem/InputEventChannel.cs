using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Nutmeg.Runtime.Utility.InputSystem
{
    [CreateAssetMenu(fileName = "InputEventChannel", menuName = "Event Channel/Input")]
    public class InputEventChannel : ScriptableObject
    {
        public Action<Vector2> onMoveActionPerformed;
        public Action<Vector2> onMoveActionCanceled;
        public Action onPrimaryActionPerformed;
        public Action onPrimaryActionCanceled;
        public Action onReloadActionPerformed;
        public Action onReloadActionCanceled;
        public Action onSecondaryActionPerformed;
        public Action onSecondaryActionCanceled;
        public Action<Vector2> onScrollActionPerformed;
        
        public void PerformScrollAction(InputAction.CallbackContext context)
        {
            onScrollActionPerformed?.Invoke(context.ReadValue<Vector2>());
        }
        
        public void PerformSecondaryAction(InputAction.CallbackContext context)
        {
            onSecondaryActionPerformed?.Invoke();
        }
        
        public void CancelSecondaryAction(InputAction.CallbackContext context)
        {
            onSecondaryActionCanceled?.Invoke();
        }
        
        public void PerformReloadAction(InputAction.CallbackContext context)
        {
            onReloadActionPerformed?.Invoke();
        }
        
        public void CancelReloadAction(InputAction.CallbackContext context)
        {
            onReloadActionCanceled?.Invoke();
        }
        
        public void PerformMoveAction(InputAction.CallbackContext context)
        {
            onMoveActionPerformed?.Invoke(context.ReadValue<Vector2>());
        }
        
        public void CancelMoveAction(InputAction.CallbackContext context)
        {
            onMoveActionCanceled?.Invoke(context.ReadValue<Vector2>());
        }
        
        public void PerformPrimaryAction(InputAction.CallbackContext context)
        {
            onPrimaryActionPerformed?.Invoke();
        }
        
        public void CancelPrimaryAction(InputAction.CallbackContext context)
        {
            onPrimaryActionCanceled?.Invoke();
        }
    }
}