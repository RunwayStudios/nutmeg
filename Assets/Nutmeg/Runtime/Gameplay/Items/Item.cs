using System;
using System.Collections;
using JetBrains.Annotations;
using Nutmeg.Runtime.Gameplay.Player;
using Unity.Netcode;
using UnityEngine;

namespace Nutmeg.Runtime.Gameplay.Items
{
    public abstract class Item : MonoBehaviour
    {
        public NetworkObject playerNetworkObject;
        
        protected virtual void Start()
        {
            playerNetworkObject = NetworkPlayerController.Main.GetComponent<NetworkObject>();
        }

        public abstract void Use();
    }
}