using System;
using UnityEngine;

namespace Nutmeg.Runtime.Gameplay.Combat
{
    public class CombatEntity : MonoBehaviour
    {
        [SerializeField] private CombatGroup group;
        
        [SerializeField] private CombatModule[] modules;
        
        
        
        // Start is called before the first frame update
        void Start()
        {
            CombatEntityManager.Main.AddCombatEntity(this, group);
            
            for (int i = 0; i < modules.Length; i++)
            {
                modules[i].InitializeModule(this);
            }
        }

        private void OnDestroy()
        {
            CombatEntityManager.Main.RemoveCombatEntity(this, group);
        }

        // Update is called once per frame
        void Update()
        {
            for (int i = 0; i < modules.Length; i++)
            {
                modules[i].UpdateModule();
            }
        }


        public bool TryGetModule(Type type, out CombatModule module)
        {
            module = null;
            for (int i = 0; i < modules.Length; i++)
            {
                if (modules[i].GetType() == type)
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
        Building
    }
}
