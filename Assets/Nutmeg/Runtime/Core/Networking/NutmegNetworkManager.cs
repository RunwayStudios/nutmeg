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
            
            base.OnServerStarted += OnServerStarted;
        }

        private void OnServerStarted()
        {
            SceneManager.OnSceneEvent += SceneManagerOnSceneEvent;
            SceneManager.OnSynchronize += SceneManagerOnSynchronize;
        }

        private void SceneManagerOnSynchronize(ulong clientid)
        {
        }

        private void SceneManagerOnSceneEvent(SceneEvent sceneEvent)
        {
            switch (sceneEvent.SceneEventType)
            {
                case SceneEventType.Load:
                    break;
                case SceneEventType.Unload:
                    break;
                case SceneEventType.Synchronize:
                    break;
                case SceneEventType.ReSynchronize:
                    break;
                case SceneEventType.LoadEventCompleted:
                    break;
                case SceneEventType.UnloadEventCompleted:
                    break;
                case SceneEventType.LoadComplete:
                    break;
                case SceneEventType.UnloadComplete:
                    break;
                case SceneEventType.SynchronizeComplete:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}