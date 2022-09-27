using System;
using System.Collections;
using Steamworks;
using UnityEngine;

namespace Nutmeg.Runtime.Core.Networking.Lobby
{
    public class SteamLobbyManager : MonoBehaviour
    {
        //private NutmegNetworkManager networkManager;

        protected Callback<LobbyCreated_t> c_lobbyCreated;
        protected Callback<LobbyEnter_t> c_lobbyEnter;
        protected Callback<LobbyInvite_t> c_lobbyInvite;
        protected Callback<LobbyChatUpdate_t> c_lobbyChatUpdate;
        protected Callback<GameLobbyJoinRequested_t> c_gameLobbyJoinRequested;

        public static CSteamID Id { get; private set; }

        private void Start()
        {
            if (!SteamManager.Initialized)
            {
                //Todo tell player no instance of steam is running
                Debug.LogError("No instance of Steam is running");
                return;
            }
            
            //networkManager = GetComponent<NutmegNetworkManager>();

            c_lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
            c_lobbyEnter = Callback<LobbyEnter_t>.Create(OnLobbyEnter);
            c_lobbyInvite = Callback<LobbyInvite_t>.Create(OnLobbyInvite);
            c_lobbyChatUpdate = Callback<LobbyChatUpdate_t>.Create(OnLobbyChatUpdate);
            c_gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);

            HostLobby(ELobbyType.k_ELobbyTypeFriendsOnly);
        }

        public void HostLobby(ELobbyType lobbyType)
        {
            //SteamMatchmaking.CreateLobby(lobbyType, networkManager.maxConnections);
        }

        private void OnLobbyCreated(LobbyCreated_t context)
        {
            switch (context.m_eResult)
            {
                case EResult.k_EResultOK:
                {
                    //networkManager.StartHost();
                    Id = new CSteamID(context.m_ulSteamIDLobby);

                    SetLobbyData(LobbyKeys.HOST_STEAM_ID, SteamUser.GetSteamID().ToString());
                    Debug.Log("Lobby created successfully");

                    break;
                }
                case EResult.k_EResultFail:
                {
                    //Todo tell client connecting to lobby failed
                    Debug.LogError("Error while creating lobby");
                    return;
                }
                case EResult.k_EResultConnectFailed:
                {
                    //Todo tell player no connection
                    return;
                }
                default:
                {
                    //Todo tell player generic error
                    return;
                }
            }
        }

        private void OnLobbyEnter(LobbyEnter_t context)
        {
            //if(NetworkServer.active) return;

            Id = new CSteamID(context.m_ulSteamIDLobby);

            string hostAddress = SteamMatchmaking.GetLobbyData(Id, LobbyKeys.HOST_STEAM_ID);

            //networkManager.networkAddress = hostAddress;
            //networkManager.StartClient();

            Debug.Log("Entered lobby");
        }

        private void OnLobbyInvite(LobbyInvite_t context)
        {
            //Todo show player invite form friend
        }

        private void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t context)
        {
            SteamMatchmaking.JoinLobby(context.m_steamIDLobby);
        }
        
        private void OnLobbyChatUpdate(LobbyChatUpdate_t context)
        {
            Debug.Log(context.m_rgfChatMemberStateChange);
        }

        public static void SetLobbyData(string key, string value) => SteamMatchmaking.SetLobbyData(Id, key, value);
    }
}