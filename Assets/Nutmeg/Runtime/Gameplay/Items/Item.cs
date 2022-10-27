using System;
using System.Collections;
using JetBrains.Annotations;
using Unity.Netcode;
using UnityEngine;

namespace Nutmeg.Runtime.Gameplay.Items
{
    public abstract class Item : MonoBehaviour
    {
        public NetworkObject playerNetworkObject;

        private void Awake()
        {
            if(TryGetComponent(typeof(NetworkObject), out var n))
                Destroy(n);
        }

        public abstract void Use();
    }
}