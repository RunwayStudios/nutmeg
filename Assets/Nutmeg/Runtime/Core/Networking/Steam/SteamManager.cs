using System;
using IngameDebugConsole;
using Netcode.Transports.Facepunch;
using Steamworks;
using Steamworks.Data;
using Unity.Netcode;
using UnityEngine;

namespace Nutmeg.Runtime.Core.Networking.Steam
{
    public class SteamManager : MonoBehaviour
    {
        public static Lobby? CurrentLobby { get; private set; }

        private FacepunchTransport transport;

        private void Awake()
        {
            transport = GetComponent<FacepunchTransport>();

            SteamMatchmaking.OnLobbyCreated += SteamMatchmakingOnLobbyCreated;
            SteamMatchmaking.OnLobbyEntered += SteamMatchmakingOnLobbyEntered;
            SteamMatchmaking.OnLobbyInvite += SteamMatchmakingOnLobbyInvite;
            SteamMatchmaking.OnLobbyMemberDisconnected += SteamMatchmakingOnLobbyMemberDisconnected;
            SteamMatchmaking.OnLobbyMemberLeave += SteamMatchmakingOnLobbyMemberLeave;
            SteamFriends.OnGameLobbyJoinRequested += SteamFriendsOnGameLobbyJoinRequested;

            DebugLogConsole.AddCommand("Steam.HostGame", "Hosts a game", HostGame);
        }

        private void SteamMatchmakingOnLobbyMemberLeave(Lobby lobby, Friend friend)
        {
            Debug.LogWarning("Lobby member " + friend.Id + " left the lobby");

            NetworkManager.Singleton.DisconnectClient(
                Convert.ToUInt64(lobby.GetMemberData(friend, SteamDataKeys.INTERNAL_NETWORK_ID)));
        }

        private void SteamMatchmakingOnLobbyMemberDisconnected(Lobby lobby, Friend friend)
        {
            Debug.LogWarning("Lobby member " + friend.Id + " lost connection");

            NetworkManager.Singleton.DisconnectClient(
                 Convert.ToUInt64(lobby.GetMemberData(friend, SteamDataKeys.INTERNAL_NETWORK_ID)));
        }

        private void SteamMatchmakingOnLobbyInvite(Friend friend, Lobby lobby)
        {
            Debug.Log(friend.Name + " invited you to a lobby " + lobby.Id);
        }

        private void SteamMatchmakingOnLobbyEntered(Lobby lobby)
        {
            Debug.Log("Entered new Steam lobby " + lobby.Id);
            transport.targetSteamId = lobby.Owner.Id;

            if (!NetworkManager.Singleton.IsHost)
            {
                NetworkManager.Singleton.StartClient();
                lobby.SetMemberData(SteamDataKeys.INTERNAL_NETWORK_ID,
                    NetworkManager.Singleton.LocalClientId.ToString());
            }
        }

        private void SteamFriendsOnGameLobbyJoinRequested(Lobby lobby, SteamId steamId)
        {
            JoinLobby(lobby.Id);
        }

        private void SteamMatchmakingOnLobbyCreated(Result result, Lobby lobby)
        {
            switch (result)
            {
                case Result.OK:
                {
                    Debug.Log("Created new Steam lobby " + lobby.Id);
                    //transport.StartServer(); 
                    break;
                }
            }
        }

        private async void JoinLobby(SteamId id)
        {
            CurrentLobby = await SteamMatchmaking.JoinLobbyAsync(id);
        }

        private async void CreateLobby()
        {
            CurrentLobby = await SteamMatchmaking.CreateLobbyAsync(4);
            CurrentLobby?.SetFriendsOnly();
        }
        
        public void HostGame()
        {
            NetworkManager.Singleton.StartHost();

            CreateLobby();
        }
    }
}