using System.Collections.Generic;
using System.Linq;
using Nutmeg.Runtime.Core.Networking.Steam;
using Nutmeg.Runtime.Core.SceneManagement;
using Nutmeg.Runtime.Gameplay.MainMenu.CharacterSelection;
using Nutmeg.Runtime.Gameplay.PlayerCharacter;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Nutmeg.Runtime.Gameplay.MainMenu.Pedestal
{
    public class MainMenuPedestalManager : NetworkBehaviour
    {
        [SerializeField] private GameObject pedestalPrefab;
        [SerializeField] private Transform[] pedestalPositions;
        private int pedestalPositionIndex;

        private Dictionary<ulong, MainMenuPedestal> pedestals = new();

        public static MainMenuPedestalManager Main { get; private set; }

        private void Awake()
        {
            Main = this;

            MainMenuCharacterSelectionManager.onPlayerCharacterSelected += OnPlayerCharacterSelected;
            NetworkManager.OnClientConnectedCallback += NetworkManagerOnClientConnectedCallback;
            NetworkManager.OnClientDisconnectCallback += NetworkManagerOnClientDisconnectCallback;
        }

        

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (NetworkManager.LocalClientId != 0)
            {
                pedestals.Add(NetworkManager.LocalClientId, pedestals[0]);
                pedestals.Remove(0);
            }
        }

        private void NetworkManagerOnClientConnectedCallback(ulong id)
        {
            if (IsServer)
            {
                foreach (var VARIABLE in pedestals)
                {
                    Debug.Log(VARIABLE.Key + " " + VARIABLE.Value);
                }

                RefreshClientClientRpc(
                    pedestals.Keys.ToArray(),
                    pedestals.Values
                        .Select(p => PlayerCharacterManager.Main.GetPlayerCharacterIndex(p.character))
                        .ToArray(),
                    new ClientRpcParams {Send = new ClientRpcSendParams {TargetClientIds = new List<ulong> {id}}}
                );

                AddPedestalClientRpc(id);
            }

            if (NetworkManager.LocalClientId == id)
                UpdatePedestal(id, PlayerCharacterManager.Main.CurrentPlayerCharacter, true);
        }
        
        private void NetworkManagerOnClientDisconnectCallback(ulong id)
        {
            if (IsServer)
            {
                RemovePedestalClientRpc(id, new ClientRpcParams {Send = new ClientRpcSendParams()});
            }
            
            RemovePedestal(id);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            MainMenuCharacterSelectionManager.onPlayerCharacterSelected -= OnPlayerCharacterSelected;
        }

        private void OnPlayerCharacterSelected(PlayerCharacter.PlayerCharacter character)
        {
            //Debug.Log(PlayerCharacterManager.Main.in);
            UpdatePedestal(NetworkManager.LocalClientId, character, true);
        }

        [ClientRpc]
        private void RefreshClientClientRpc(ulong[] ids, int[] characterIndexes, ClientRpcParams par)
        {
            for (int i = 0; i < ids.Length; i++)
                AddPedestal(ids[i], PlayerCharacterManager.Main.GetPlayerCharacter(characterIndexes[i]));

            Debug.Log(pedestals.Count);
        }

        [ServerRpc(RequireOwnership = false)]
        private void UpdatePedestalServerRpc(ulong id, int characterIndex, ServerRpcParams args = default)
        {
            UpdatePedestalClientRpc(id, characterIndex,
                new ClientRpcParams
                {
                    Send = new ClientRpcSendParams
                    {
                        TargetClientIds = NetworkManager.ConnectedClientsIds
                            .Where(i => i != args.Receive.SenderClientId).ToList()
                    }
                });
        }

        [ClientRpc]
        private void UpdatePedestalClientRpc(ulong id, int characterIndex, ClientRpcParams args)
        {
            if (!PlayerCharacterManager.Main.TryGetPlayerCharacter(characterIndex, out var c))
                return;

            UpdatePedestal(id, c, false);
        }

        public void UpdatePedestal(ulong id, PlayerCharacter.PlayerCharacter character, bool sync)
        {
            Debug.Log($"update pedestal {id}");

            pedestals[id].UpdatePlayerCharacter(character);

            if (IsClient && sync)
                UpdatePedestalServerRpc(id, PlayerCharacterManager.Main.GetPlayerCharacterIndex(character));
        }

        [ClientRpc]
        private void AddPedestalClientRpc(ulong id)
        {
            AddPedestal(id);
        }

        public void AddPedestal(ulong id, PlayerCharacter.PlayerCharacter character = null)
        {
            Debug.Log($"add pedestal {id}");

            if (pedestals.ContainsKey(id))
                return;

            MainMenuPedestal pedestal =
                Instantiate(pedestalPrefab, pedestalPositions[pedestalPositionIndex++])
                    .GetComponent<MainMenuPedestal>();
            pedestal.Initialize(character);
            pedestal.gameObject.name = "Pedestal (" + id + ")";
            pedestals.Add(id, pedestal);
        }

        [ClientRpc]
        private void RemovePedestalClientRpc(ulong id, ClientRpcParams args)
        {
            RemovePedestal(id);
        }

        public void RemovePedestal(ulong id)
        {
            pedestalPositionIndex--;
            pedestals[id].Remove();
            pedestals.Remove(id);
        }
    }
}