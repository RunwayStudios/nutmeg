using System;
using System.Collections.Generic;
using Nutmeg.Runtime.Core.GameManager;
using Nutmeg.Runtime.Core.Networking.Steam;
using Nutmeg.Runtime.Gameplay.MainMenu.CharacterSelection;
using Steamworks;
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

        private void Start()
        {
            Main = this;
            
            MainMenuCharacterSelectionManager.onPlayerCharacterSelected += OnPlayerCharacterSelected;
            SteamManager.onSteamLobbyEntered += OnSteamLobbyEntered;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            MainMenuCharacterSelectionManager.onPlayerCharacterSelected -= OnPlayerCharacterSelected;
        }
        
        private void OnSteamLobbyEntered()
        {
        }

        private void OnPlayerCharacterSelected(PlayerCharacter.PlayerCharacter character)
        {
            UpdatePedestal(SteamManager.Id, character);
        }

        [ServerRpc]
        private void AddPedestalServerRpc(ulong id, int characterIndex) => AddPedestalClientRpc(id, characterIndex);

        [ClientRpc]
        private void AddPedestalClientRpc(ulong id, int characterIndex)
        {
            MainMenuCharacterSelectionManager.Main.TryGetPlayerCharacter(characterIndex, out var c);
            AddPedestal(id, c);
        }
        
        public void UpdatePedestal(ulong id, PlayerCharacter.PlayerCharacter character)
        {
            pedestals[id].UpdatePlayerCharacter(character);
        }

        public void AddPedestal(ulong id, PlayerCharacter.PlayerCharacter character)
        {
            if(pedestals.ContainsKey(id))
                return;
            
            MainMenuPedestal pedestal =
                Instantiate(pedestalPrefab, pedestalPositions[pedestalPositionIndex++]).GetComponent<MainMenuPedestal>();
            pedestal.Initialize(character);
            pedestals.Add(id, pedestal);
        }

        public void RemovePedestal()
        {
            pedestalPositionIndex--;
        }
    }
}