using Nutmeg.Runtime.Core.Networking.Steam;
using Steamworks;
using TMPro;
using UnityEngine;

namespace Nutmeg.Runtime.UI.FriendsList
{
    public class FriendListItem : MonoBehaviour
    {
        [SerializeField] private TMP_Text friendNameText;

        [SerializeField] private Color isPlayingThisGameColor;
        [SerializeField] private Color isOnlineColor;
        [SerializeField] private Color isAwayColor;
        [SerializeField] private Color isOfflineColor;

        private Friend friend;
        private float friendDataCooldown;

        public void Initialize(Friend friend)
        {
            this.friend = friend;
            friendNameText.text = friend.Name;
            SetFriendStatus();
        }

        private void SetFriendStatus()
        {
            if (friend.IsPlayingThisGame)
            {
                friendNameText.color = isPlayingThisGameColor;
                return;
            }

            if (friend.IsOnline)
            {
                friendNameText.color = isOnlineColor;
                return;
            }

            if (friend.IsAway)
            {
                friendNameText.color = isAwayColor;
                return;
            }

            friendNameText.color = isOfflineColor;
        }

        public void InviteFriend() => SteamManager.Main.SendLobbyInvite(friend.Id);
    }
}