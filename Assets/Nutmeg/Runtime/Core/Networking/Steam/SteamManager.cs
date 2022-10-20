using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public static SteamId Id => SteamClient.SteamId;
        
        public static SteamManager Main { get; private set; }

        public static Action onSteamLobbyEntered;
        public static Action onSteamLobbyCreated;
        public static Action<Friend> onFriendJoinedSteamLobby;
        public static Action<Friend> onFriendLeftSteamLobby;
        public static Action<Friend> onLobbyMemberDataChanged;

        private void Awake()
        {
            Main = this;

            transport = GetComponent<FacepunchTransport>();

            SteamMatchmaking.OnLobbyCreated += SteamMatchmakingOnLobbyCreated;
            SteamMatchmaking.OnLobbyEntered += SteamMatchmakingOnLobbyEntered;
            SteamMatchmaking.OnLobbyInvite += SteamMatchmakingOnLobbyInvite;
            SteamMatchmaking.OnLobbyMemberDataChanged += SteamMatchmakingOnLobbyMemberDataChanged;
            SteamMatchmaking.OnLobbyMemberJoined += SteamMatchmakingOnLobbyMemberJoined;
            SteamMatchmaking.OnLobbyMemberLeave += SteamMatchmakingOnLobbyMemberLeave;
            SteamMatchmaking.OnLobbyMemberDisconnected += SteamMatchmakingOnLobbyMemberDisconnected;
            SteamFriends.OnGameLobbyJoinRequested += SteamFriendsOnGameLobbyJoinRequested;

            DebugLogConsole.AddCommand("Steam.HostGame", "Hosts a game", HostGame);
            DebugLogConsole.AddCommand("Steam.CreateLobby", "Create a new steam lobby", CreateSteamLobby);
        }

        

        private void OnDestroy()
        {
            SteamMatchmaking.OnLobbyCreated -= SteamMatchmakingOnLobbyCreated;
            SteamMatchmaking.OnLobbyEntered -= SteamMatchmakingOnLobbyEntered;
            SteamMatchmaking.OnLobbyInvite -= SteamMatchmakingOnLobbyInvite;
            SteamMatchmaking.OnLobbyMemberDataChanged -= SteamMatchmakingOnLobbyMemberDataChanged;
            SteamMatchmaking.OnLobbyMemberJoined -= SteamMatchmakingOnLobbyMemberJoined;
            SteamMatchmaking.OnLobbyMemberLeave -= SteamMatchmakingOnLobbyMemberLeave;
            SteamMatchmaking.OnLobbyMemberDisconnected -= SteamMatchmakingOnLobbyMemberDisconnected;
            SteamFriends.OnGameLobbyJoinRequested -= SteamFriendsOnGameLobbyJoinRequested;

            DebugLogConsole.RemoveCommand(HostGame);
            DebugLogConsole.RemoveCommand(CreateSteamLobby);
        }
        
        public async void HostGame()
        {
            NetworkManager.Singleton.StartHost();
            
            await CreateSteamLobby();
        }

        #region SteamLobby

        public static void SetLocalLobbyMemberData(string key, string value) => CurrentLobby?.SetMemberData(key, value);

        public static void SetLobbyMetaData(string key, string value) => CurrentLobby?.SetData(key, value);

        public static string GetLobbyMemberData(Friend friend, string key) => CurrentLobby?.GetMemberData(friend, key);

        public static string GetLobbyMetaData(string key) => CurrentLobby?.GetData(key);

        public async void SendLobbyInvite(SteamId id)
        {
            if (CurrentLobby == null)
                await CreateSteamLobby();

            Debug.Log("Send invite to " + id);
            
            CurrentLobby?.InviteFriend(id);
        }

        private async void JoinLobby(SteamId id)
        {
            CurrentLobby = await SteamMatchmaking.JoinLobbyAsync(id);
        }

        private async Task CreateSteamLobby()
        {
            //TODO max members should be max allowed connections
            CurrentLobby = await SteamMatchmaking.CreateLobbyAsync(4);
            CurrentLobby?.SetFriendsOnly();
        }
        
        private void SteamMatchmakingOnLobbyMemberDataChanged(Lobby lobby, Friend friend)
        {
            if(lobby.Id != CurrentLobby?.Id)
                return;
            
            onLobbyMemberDataChanged?.Invoke(friend);
        }

        private void SteamMatchmakingOnLobbyMemberJoined(Lobby lobby, Friend friend) => onFriendJoinedSteamLobby?.Invoke(friend);

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

        /// <summary>
        /// Event for when client gets invite from a friend
        /// </summary>
        /// <param name="friend"></param>
        /// <param name="lobby"></param>
        private void SteamMatchmakingOnLobbyInvite(Friend friend, Lobby lobby)
        {
            Debug.Log(friend.Name + " invited you to a lobby " + lobby.Id);
        }

        private void SteamMatchmakingOnLobbyEntered(Lobby lobby)
        {
            Debug.Log("Entered new Steam lobby " + lobby.Id);
            transport.targetSteamId = lobby.Owner.Id;

            onSteamLobbyEntered?.Invoke();
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
                    NetworkManager.Singleton.StartHost();
                    break;
                }
            }
        }
        
        public static int FriendSortByActivity(Friend a, Friend b)
        {
            if (a.IsPlayingThisGame && b.IsPlayingThisGame || a.State == b.State)
                return string.Compare(a.Name, b.Name, StringComparison.CurrentCulture);

            if (a.IsPlayingThisGame)
                return -1;
            if (b.IsPlayingThisGame)
                return 1;

            if (a.IsOnline)
                return -1;
            if (b.IsOnline)
                return 1;

            if (a.IsAway)
                return -1;
            if (b.IsAway)
                return 1;
            return -1;
        }

        #endregion

        #region SteamFriends

        public static int GetSteamFriendCount => SteamFriends.GetFriends().Count();

        public static Friend[] GetSteamFriends() => SteamFriends.GetFriends().ToArray();

        public static Friend[] GetSteamFriendsSorted()
        {
            List<Friend> list = new(SteamFriends.GetFriends().ToList());
            list.Sort(FriendSortByActivity);
            return list.ToArray();
        }

        #endregion
    }
}