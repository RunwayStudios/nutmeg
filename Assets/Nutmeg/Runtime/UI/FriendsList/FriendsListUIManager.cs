using System.Collections.Generic;
using Nutmeg.Runtime.Core.Networking.Steam;
using UnityEngine;

namespace Nutmeg.Runtime.UI.FriendsList
{
    public class FriendsListUIManager : MonoBehaviour
    {
        [SerializeField] private GameObject friendListItemUIPrefab;
        [SerializeField] private RectTransform listContent;
        [SerializeField] private float itemPadding;
        [SerializeField] private List<GameObject> friendListItems;

        private void Start()
        {
            RefreshFriendsList();
        }

        private void RefreshFriendsList()
        {
            ClearFriendsList();

            listContent.sizeDelta = Vector2.up * (SteamManager.GetSteamFriendCount *
                                                  friendListItemUIPrefab.GetComponent<RectTransform>().rect.height +
                                                  itemPadding * SteamManager.GetSteamFriendCount);

            foreach (var friend in SteamManager.GetSteamFriendsSorted())
            {
                Instantiate(friendListItemUIPrefab, listContent).GetComponent<FriendListItem>().Initialize(friend);
            }
        }

        private void ClearFriendsList()
        {
            foreach (var friendListItem in friendListItems)
            {
                friendListItems.Remove(friendListItem);
                Destroy(friendListItem);
            }
        }
    }
}