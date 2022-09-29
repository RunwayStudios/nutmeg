using Unity.Netcode;
using UnityEngine.InputSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using Nutmeg.Runtime.Gameplay.Items;
using Nutmeg.Runtime.Utility.InputSystem;
using Nutmeg.Runtime.Utility.MouseController;
using UnityEngine;

namespace Nutmeg.Runtime.Gameplay.Player
{
    public class NetworkPlayerController : NetworkBehaviour
    {
        [SerializeField] private float moveSpeed;
        [SerializeField] private Transform playerBody;
        [SerializeField] private Item weapon;


        private InputActions input;
        private Action perFrameActions;

        private CharacterController cc;

        private Vector2 rawMoveVector;

        private bool movePlayer;

        public bool IsMoving { get; private set; }

        private void Start()
        {
            if (!IsLocalPlayer) return;
            cc = GetComponent<CharacterController>();


            perFrameActions += RotatePlayer;

            input = InputManager.Input;


            input.Player.Move.performed += OnMoveActionPerformed;
            input.Player.Move.canceled += OnMoveActionCanceled;

            input.Player.Primary.performed += OnPrimaryActionPerformed;
            input.Player.Primary.canceled += OnPrimaryActionCanceled;
        }

        private void OnPrimaryActionPerformed(InputAction.CallbackContext context)
        {
            perFrameActions += PrimaryAction;
        }

        private void OnPrimaryActionCanceled(InputAction.CallbackContext context)
        {
            perFrameActions -= PrimaryAction;
        }

        private void Update()
        {
            perFrameActions?.Invoke();
        }

        private void OnMoveActionPerformed(InputAction.CallbackContext context)
        {
            if (!IsMoving)
                perFrameActions += MovePlayer;

            IsMoving = true;

            rawMoveVector = context.ReadValue<Vector2>();
        }

        private void OnMoveActionCanceled(InputAction.CallbackContext context)
        {
            perFrameActions -= MovePlayer;
            IsMoving = false;
        }

        private void MovePlayer()
        {
            //transform.position = new Vector3(3f, 1f, 0f);

            //Destroy(gameObject);

            cc.Move((Vector3.left * rawMoveVector.x + Vector3.back * rawMoveVector.y) * moveSpeed *
                    Time.deltaTime);
            //MovePlayerClientRpc((Vector3.left * rawMoveVector.x + Vector3.back * rawMoveVector.y) * moveSpeed *
            //      Time.deltaTime);
        }

        private void RotatePlayer()
        {
            playerBody.LookAt(MouseController.GetLastMouseLookTargetPoint());
            playerBody.rotation = Quaternion.Euler(0f, playerBody.rotation.eulerAngles.y, 0f);
        }

        private void PrimaryAction()
        {
            //Use Item
            weapon.Use();

            List<ulong> ids = NetworkManager.Singleton.ConnectedClientsIds.ToList();
            ids.Remove(NetworkManager.Singleton.LocalClientId);

            if (IsLocalPlayer)
            {
                PrimaryActionClientRpc(new ClientRpcParams {Send = new ClientRpcSendParams {TargetClientIds = ids}});
            }
        }

        [ClientRpc]
        private void PrimaryActionClientRpc(ClientRpcParams param) => PrimaryAction();
    }
}