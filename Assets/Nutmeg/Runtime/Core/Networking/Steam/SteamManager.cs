using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Nutmeg.Runtime.Core.Networking.Steam
{
    public class SteamManager : MonoBehaviour
    {
        private void Awake()
        {
            //TODO temporary REMOVE
            Steamworks.SteamClient.Shutdown();
            
            try
            {
                Steamworks.SteamClient.Init(480);
            }
            catch (Exception)
            {
                // Something went wrong - it's one of these:
                //
                //     Steam is closed?
                //     Can't find steam_api dll?
                //     Don't have permission to play app?
                //

                Debug.LogError("Failed to init SteamClient");
            }
        }

        private void OnApplicationQuit()
        {
            Steamworks.SteamClient.Shutdown();
        }
    }
}