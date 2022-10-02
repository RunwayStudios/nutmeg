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
        public static Lobby Lobby { get; private set; }

        private FacepunchTransport transport;

        private void Awake()
        {
            transport = GetComponent<FacepunchTransport>();

            SteamMatchmaking.OnLobbyCreated += SteamMatchmakingOnLobbyCreated;
            SteamMatchmaking.OnLobbyEntered += SteamMatchmakingOnLobbyEntered;
            SteamMatchmaking.OnLobbyInvite += SteamMatchmakingOnLobbyInvite;
            SteamMatchmaking.OnLobbyMemberDisconnected += SteamMatchmakingOnLobbyMemberDisconnected;
            SteamFriends.OnGameLobbyJoinRequested += SteamFriendsOnGameLobbyJoinRequested;

            DebugLogConsole.AddCommand("Steam.CreateLobby", "Create a steam Lobby", CreateLobby);
        }

        private void SteamMatchmakingOnLobbyMemberDisconnected(Lobby lobby, Friend friend)
        {
            Debug.LogWarning("Lobby member " + friend.Id + " lost connection");
            
            NetworkManager.Singleton.DisconnectClient(
                Convert.ToUInt64(lobby.GetMemberData(friend, SteamDataKeys.INTERNAL_NETWORK_ID)));
        }

        private void SteamMatchmakingOnLobbyInvite(Friend friend, Lobby lobby)
        {
            throw new NotImplementedException();
        }

        private void SteamMatchmakingOnLobbyEntered(Lobby lobby)
        {
            Debug.Log("Entered new Steam lobby " + lobby.Id);

            transport.targetSteamId = lobby.Owner.Id;
            Lobby = lobby;

            if (!NetworkManager.Singleton.IsHost)
            {
                //transport.StartClient();
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
                    NetworkManager.Singleton.StartHost();
                    break;
                }
            }
        }

        private void JoinLobby(SteamId id)
        {
            SteamMatchmaking.JoinLobbyAsync(id);
        }

        public void CreateLobby()
        {
            SteamMatchmaking.CreateLobbyAsync(4);
        }
    }
}