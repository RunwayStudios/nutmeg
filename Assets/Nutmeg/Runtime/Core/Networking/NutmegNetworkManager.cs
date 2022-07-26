using System.Collections.Generic;
using Mirror;
using Nutmeg.Runtime.Core.Networking.Lobby;
using Steamworks;
using UnityEngine;
using UnityEngine.UI;

namespace Nutmeg.Runtime.Core.Networking
{
    public class NutmegNetworkManager : NetworkManager
    {
        [SerializeField] private Text teyt;
        [SerializeField] private Transform startPosition;

        private void Start()
        {
            startPositions = new List<Transform> {startPosition};
        }


        public override void OnClientConnect()
        {
            base.OnClientConnect();
        }
    }
}