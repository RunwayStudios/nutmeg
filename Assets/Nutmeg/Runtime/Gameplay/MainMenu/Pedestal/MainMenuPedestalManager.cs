using System.Collections.Generic;
using System.Linq;
using Nutmeg.Runtime.Core.Networking.Steam;
using Nutmeg.Runtime.Gameplay.MainMenu.CharacterSelection;
using Nutmeg.Runtime.Gameplay.PlayerCharacter;
using Unity.Netcode;
using UnityEngine;

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
        }

        private void NetworkManagerOnClientConnectedCallback(ulong id)
        {
            /*if (IsServer)
            {
                foreach (var VARIABLE in pedestals)
                {
                    Debug.Log(VARIABLE.Key + " " + VARIABLE.Value);
                }
                
                /*RefreshClientClientRpc(
                    pedestals.Keys.ToArray(),
                    pedestals.Values
                        .Select(p => PlayerCharacterManager.Main.GetPlayerCharacterIndex(p.character))
                        .ToArray(),
                    new ClientRpcParams {Send = new ClientRpcSendParams {TargetClientIds = new List<ulong> {id}}}
                );
                AddPedestalClientRpc(id);
            }

            if (NetworkManager.LocalClientId == id)
                UpdatePedestalServerRpc(id, PlayerCharacterManager.Main.CharacterIndex);*/
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            MainMenuCharacterSelectionManager.onPlayerCharacterSelected -= OnPlayerCharacterSelected;
        }

        private void OnPlayerCharacterSelected(PlayerCharacter.PlayerCharacter character)
        {
            //Debug.Log(PlayerCharacterManager.Main.in);
            UpdatePedestal(SteamManager.Id, character);
        }

        [ClientRpc]
        private void AddPedestalClientRpc(ulong id)
        {
            AddPedestal(id);
        }

        [ServerRpc(RequireOwnership = false)]
        private void UpdatePedestalServerRpc(ulong id, int characterIndex) =>
            UpdatePedestalClientRpc(id, characterIndex);

        [ClientRpc]
        private void RefreshClientClientRpc(ulong[] ids, int[] characterIndexes, ClientRpcParams par)
        {
        }

        [ClientRpc]
        private void UpdatePedestalClientRpc(ulong id, int characterIndex)
        {
            if (!PlayerCharacterManager.Main.TryGetPlayerCharacter(characterIndex, out var c))
                return;

            UpdatePedestal(id, c);
        }

        public void UpdatePedestal(ulong id, PlayerCharacter.PlayerCharacter character)
        {
            pedestals[id].UpdatePlayerCharacter(character);

            if (IsClient)
                UpdatePedestalClientRpc(id, PlayerCharacterManager.Main.GetPlayerCharacterIndex(character));
        }

        public void AddPedestal(ulong id, PlayerCharacter.PlayerCharacter character = null)
        {
            if (pedestals.ContainsKey(id))
                return;

            MainMenuPedestal pedestal =
                Instantiate(pedestalPrefab, pedestalPositions[pedestalPositionIndex++])
                    .GetComponent<MainMenuPedestal>();
            pedestal.Initialize(character);
            pedestal.gameObject.name = "Pedestal (" + id + ")";
            pedestals.Add(id, pedestal);
        }

        public void RemovePedestal()
        {
            pedestalPositionIndex--;
        }
    }
}