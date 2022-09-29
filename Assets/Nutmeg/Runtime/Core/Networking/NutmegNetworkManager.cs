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
            DebugLogConsole.AddCommand("StartHost", "Start as host", StartHost);
            DebugLogConsole.AddCommand("StartClient", "Start as client", StartClient);
            DebugLogConsole.AddCommand("HostServer", "Start as server", StartServer);
            DebugLogConsole.AddCommand("Disconnect", "Disconnect from the server",
                () => DisconnectClient(LocalClientId));
            DebugLogConsole.AddCommand("Shutdown", "Shutdown Server", () => Shutdown());
        }
    }
}