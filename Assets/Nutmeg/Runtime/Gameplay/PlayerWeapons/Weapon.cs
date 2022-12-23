using System;
using Unity.Netcode;
using UnityEngine;

namespace Nutmeg.Runtime.Gameplay.PlayerWeapons
{
    public class Weapon : NetworkBehaviour
    {
        [SerializeField] private WeaponModule[] modules;


        
        void Start()
        {
            for (int i = 0; i < modules.Length; i++)
            {
                modules[i].InitializeModule(this, null);
            }
        }

        void Update()
        {
            for (int i = 0; i < modules.Length; i++)
            {
                modules[i].UpdateModule();
            }
        }

        private void OnDestroy()
        {
            for (int i = 0; i < modules.Length; i++)
            {
                modules[i].DestroyModule();
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            modules = GetComponents<WeaponModule>();
        }
#endif


        public bool TryGetModule(Type type, out WeaponModule module)
        {
            for (int i = 0; i < modules.Length; i++)
            {
                Type curType = modules[i].GetType();
                if (curType == type || type.IsAssignableFrom(curType))
                {
                    module = modules[i];
                    return true;
                }
            }

            module = null;
            return false;
        }
    }
}
