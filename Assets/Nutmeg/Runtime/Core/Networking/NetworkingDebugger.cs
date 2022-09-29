using System;
using Nutmeg.Runtime.Core.Networking.Steam;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace Nutmeg.Runtime.Core.Networking
{
    public class NetworkingDebugger : MonoBehaviour
    {
        [SerializeField] private Text lobbyID;
        [SerializeField] private Text lobbyMember;
        [SerializeField] private Button hostButton;
        [SerializeField] private Button serverButton;
        [SerializeField] private Button clientButton;

        private void Start()
        {
            hostButton.onClick.AddListener(() =>
            {
                NetworkManager.Singleton.StartHost();
            }); 
            serverButton.onClick.AddListener(() =>
            {
                NetworkManager.Singleton.StartServer();
            }); 
            clientButton.onClick.AddListener(() =>
            {
                NetworkManager.Singleton.StartClient();
            }); 
        }
    }
}
