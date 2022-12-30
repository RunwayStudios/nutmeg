using Unity.Netcode;
using UnityEngine.InputSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using IngameDebugConsole;
using Nutmeg.Runtime.Gameplay.Combat.PlayerWeapons;
using Nutmeg.Runtime.Gameplay.PlayerWeapons;
using Nutmeg.Runtime.Utility.InputSystem;
using Nutmeg.Runtime.Utility.MouseController;
using UnityEngine;

namespace Nutmeg.Runtime.Gameplay.Player
{
    public class NetworkPlayerController : NetworkBehaviour
    {
        [SerializeField] private float moveSpeed;
        [SerializeField] private Transform playerBody;
        
        [SerializeField] private WeaponParent weaponParent;
        [SerializeField] private Weapon weapon;
        [SerializeField] private GameObject[] weapons;
        
        public static NetworkPlayerController Main { get; private set; }

        private InputActions input;
        private Action perFrameActions;

        private CharacterController cc;

        private Vector2 rawMoveVector;

        private bool movePlayer;

        public bool IsMoving { get; private set; }

        private void Start()
        {
            if (!IsLocalPlayer) return;
            Main = this;

            cc = GetComponent<CharacterController>();

            //TODO ???
            Debug.developerConsoleVisible = true;

            perFrameActions += RotatePlayer;

            input = InputManager.Input;


            input.Player.Move.performed += OnMoveActionPerformed;
            input.Player.Move.canceled += OnMoveActionCanceled;

            input.Player.Primary.performed += OnPrimaryActionPerformed;
            input.Player.Primary.canceled += OnPrimaryActionCanceled;


            DebugLogConsole.AddCommand("Player.SelectWeapon.none", "Select Weapon", DeselectWeapon);
            foreach (GameObject weapon1 in weapons)
            {
                DebugLogConsole.AddCommand("Player.SelectWeapon." + weapon1.name, "Select Weapon", () => CommandSelectWeapon(weapon1.name));
            }
        }

        private void CommandSelectWeapon(string newWeapon)
        {
            foreach (GameObject weapon1 in weapons)
            {
                if (weapon1.name != newWeapon)
                    continue;

                SelectWeapon(weapon1);
                return;
            }
        }

        private void SelectWeapon(GameObject newWeapon)
        {
            // TODO pooling
            if (weapon)
                Destroy(weapon.gameObject);
            weapon = Instantiate(newWeapon.gameObject).GetComponent<Weapon>();
            weapon.GetComponent<NetworkObject>().Spawn();
            weaponParent.SetWeapon(weapon);
        }

        private void DeselectWeapon()
        {
            // TODO pooling
            Destroy(weapon.gameObject);
            weapon = null;
            weaponParent.SetWeapon(null);
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

            cc.Move((Vector3.left * rawMoveVector.x + Vector3.back * rawMoveVector.y + Vector3.down * .4f) * moveSpeed *
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
            // weapon.playerNetworkObject = GetComponent<NetworkObject>();
            // weapon.Use();

            if (IsLocalPlayer)
            {
                //PrimaryActionServerRpc(NetworkManager.Singleton.LocalClientId);   
            }
        }

        [ServerRpc]
        private void PrimaryActionServerRpc(ulong id)
        {
            List<ulong> ids = NetworkManager.Singleton.ConnectedClientsIds.ToList();
            ids.Remove(id);

            PrimaryActionClientRpc(new ClientRpcParams { Send = new ClientRpcSendParams { TargetClientIds = ids } });
        }

        [ClientRpc]
        private void PrimaryActionClientRpc(ClientRpcParams param) => PrimaryAction();
    }
}