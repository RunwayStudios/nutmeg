using System;
using UnityEngine;

namespace Nutmeg.Runtime.Core.Networking.Steam
{
    public class SteamManager : MonoBehaviour
    {
        private void Start()
        {
            try
            {
                Steamworks.SteamClient.Init(480);
            }
            catch (System.Exception e)
            {
                // Something went wrong - it's one of these:
                //
                //     Steam is closed?
                //     Can't find steam_api dll?
                //     Don't have permission to play app?
                //

                Debug.LogError("Something went wring while starting SteamClient");
            }
        }

        private void Update()
        {
            Steamworks.SteamClient.RunCallbacks();
        }

        private void OnDestroy()
        {
            Steamworks.SteamClient.Shutdown();
        }
    }
}