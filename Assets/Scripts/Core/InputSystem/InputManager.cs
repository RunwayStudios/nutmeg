using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityTemplateProjects.Core.InputSystem;

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
        input.Player.Enable();
    }

    private void OnDestroy()
    {
        input.Player.Move.performed -= MoveOnPerformed;
        input.Player.Move.canceled -= MoveOnCanceled;
        input.Player.Fire.performed -= FireOnPerformed;
        input.Player.Fire.canceled -= FireOnCanceled;
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