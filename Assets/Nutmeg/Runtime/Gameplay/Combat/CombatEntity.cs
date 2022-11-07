using System;
using UnityEngine;

namespace Nutmeg.Runtime.Gameplay.Combat
{
    public class CombatEntity : MonoBehaviour
    {
        [SerializeField] private CombatGroup group;

        [SerializeField] private CombatModule[] modules;


        
        void Start()
        {
            CombatEntityManager.Main.AddCombatEntity(this, group);

            for (int i = 0; i < modules.Length; i++)
            {
                modules[i].InitializeModule(this);
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
            OnDeath();
        }

        public void OnDeath()
        {
            CombatEntityManager.Main.RemoveCombatEntity(this, group);
            enabled = false;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            modules = GetComponents<CombatModule>();
        }
#endif


        public bool TryGetModule(Type type, out CombatModule module)
        {
            module = null;
            for (int i = 0; i < modules.Length; i++)
            {
                Type curType = modules[i].GetType();
                if (curType == type || type.IsAssignableFrom(curType))
                {
                    module = modules[i];
                    return true;
                }
            }

            return false;
        }


        public CombatGroup Group => group;
    }


    public enum CombatGroup
    {
        Zombie,
        Building,
        Player
    }
}