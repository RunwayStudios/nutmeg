using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Nutmeg.Runtime.Utility.InputSystem
{
    public class InputManager : MonoBehaviour
    {
        public static InputActions Input { get; private set; }
        public static InputManager Main { get; private set; }


        [SerializeField] private InputEventChannel eventChannel;

        [SerializeField] private List<InputState> inputLayers;
        [SerializeField] private InputState initialBaseLayer;


        private void OnEnable()
        {
            Main = this;
            Input = new InputActions();

            SetBaseLayer(initialBaseLayer);


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
        }

        private void Start()
        {
        }

        public void SetBaseLayer(InputState state)
        {
            if (inputLayers.Count < 1)
                inputLayers.Add(initialBaseLayer);
            else
                inputLayers[0] = initialBaseLayer;

            ApplyInputLayers();
        }

        public void AddLayer(InputState state)
        {
            inputLayers.Add(state);
            ApplyInputLayers();
        }

        public void RemoveLayers(int count)
        {
            if (count < 1)
                return;

            if (count > inputLayers.Count - 1)
            {
                ResetToBaseLayer();
                return;
            }

            inputLayers.RemoveRange(inputLayers.Count - 1 - count, count);
            ApplyInputLayers();
        }

        public void ResetToBaseLayer()
        {
            RemoveLayers(inputLayers.Count - 1);
        }

        public void RemoveLayersAboveTarget(InputState newTargetState)
        {
            int highestIndexFound = -1;
            for (int i = 0; i < inputLayers.Count; i++)
            {
                if (inputLayers[i] == newTargetState)
                    highestIndexFound = i;
            }

            if (highestIndexFound == -1)
                return;

            RemoveLayers(inputLayers.Count - 1 - highestIndexFound);
        }

        public void RemoveLayer()
        {
            RemoveLayers(1);
        }

        public void RemoveLayer(InputState target)
        {
            while (inputLayers.Contains(target))
            {
                inputLayers.Remove(target);
            }

            ApplyInputLayers();
        }

        private void ApplyInputLayers()
        {
            IEnumerator<InputAction> inputActions = Input.GetEnumerator();

            while (inputActions.MoveNext())
            {
                ApplyInputLayers(inputActions.Current);
            }

            inputActions.Dispose();
        }

        private void ApplyInputLayers(InputAction inputAction)
        {
            Guid id = inputAction.id;
            // iterate through all layers in reverse order to prevent unnecessary enabling/disabling
            for (int i = inputLayers.Count - 1; i >= 0; i--)
            {
                if (inputLayers[i].inputIgnores[id])
                    continue;

                if (inputLayers[i].inputs[id])
                    inputAction.Enable();
                else
                    inputAction.Disable();

                // return after finding the last (only relevant) specification
                return;
            }

            // if no layer specifies a value, we default to disable
            inputAction.Disable();
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