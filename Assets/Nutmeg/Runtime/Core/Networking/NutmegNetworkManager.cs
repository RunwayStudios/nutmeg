using System;
using IngameDebugConsole;
using Unity.Netcode;
using UnityEngine;

namespace Nutmeg.Runtime.Core.Networking
{
    public class NutmegNetworkManager : NetworkManager
    {
        private void Start()
        {
            DebugLogConsole.AddCommand("Networking.StartHost", "Start as host", StartHost);
            DebugLogConsole.AddCommand("Networking.StartClient", "Start as client", StartClient);
            DebugLogConsole.AddCommand("Networking.HostServer", "Start as server", StartServer);
            DebugLogConsole.AddCommand("Networking.Disconnect", "Disconnect from the server",
                () => DisconnectClient(LocalClientId));
            DebugLogConsole.AddCommand("Networking.Shutdown", "Shutdown Server", () => Shutdown());
        }
    }
}