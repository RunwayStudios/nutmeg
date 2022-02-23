using System;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

    [CreateAssetMenu(fileName = "InputEventChannel", menuName = "Event Channel/Input")]
    public class InputEventChannel : ScriptableObject
    {
        public Action<Vector2> onMoveActionPerformed;
        public Action<Vector2> onMoveActionCanceled;
        public Action onFireActionPerformed;
        public Action onFireActionCanceled;
        public Action onReloadActionPerformed;
        public Action onReloadActionCanceled;
        public Action onThrowActionPerformed;
        public Action onThrowActionCanceled;
        public Action<Vector2> onScrollActionPerformed;
        
        public void PerformScrollAction(InputAction.CallbackContext context)
        {
            onScrollActionPerformed?.Invoke(context.ReadValue<Vector2>());
        }
        
        public void PerformThrowAction(InputAction.CallbackContext context)
        {
            onThrowActionPerformed?.Invoke();
        }
        
        public void CancelThrowAction(InputAction.CallbackContext context)
        {
            onThrowActionCanceled?.Invoke();
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
        
        public void PerformFireAction(InputAction.CallbackContext context)
        {
            onFireActionPerformed?.Invoke();
        }
        
        public void CancelFireAction(InputAction.CallbackContext context)
        {
            onFireActionCanceled?.Invoke();
        }
    }