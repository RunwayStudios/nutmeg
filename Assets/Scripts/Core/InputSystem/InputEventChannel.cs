using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UnityTemplateProjects.Core.InputSystem
{
    [CreateAssetMenu(fileName = "InputEventChannel", menuName = "Event Channel/Input")]
    public class InputEventChannel : ScriptableObject
    {
        public Action<Vector2> onMoveActionPerformed;
        public Action<Vector2> onMoveActionCanceled;
        public Action onFireActionPerformed;
        public Action onFireActionCanceled;
        
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
}