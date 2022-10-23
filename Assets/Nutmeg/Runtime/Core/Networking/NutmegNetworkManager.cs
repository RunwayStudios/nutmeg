using System;
using IngameDebugConsole;
using Nutmeg.Runtime.Gameplay.Player;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Nutmeg.Runtime.Core.Networking
{
    public class NutmegNetworkManager : NetworkManager
    {
        public int maxConnections;

        public static NutmegNetworkManager Main { get; private set; }

        private void Start()
        {
            Main = this;
            
            DebugLogConsole.AddCommand("Networking.StartHost", "Start as host", StartHost);
            DebugLogConsole.AddCommand("StartHost", "Start as host", StartHost);
            DebugLogConsole.AddCommand("Networking.StartClient", "Start as client", StartClient);
            DebugLogConsole.AddCommand("StartClient", "Start as client", StartClient);
            DebugLogConsole.AddCommand("Networking.HostServer", "Start as server", StartServer);
            DebugLogConsole.AddCommand("Networking.Disconnect", "Disconnect from the server",
                () => DisconnectClient(LocalClientId));
            DebugLogConsole.AddCommand("Networking.Shutdown", "Shutdown Server", () => Shutdown());
        }
    }
}