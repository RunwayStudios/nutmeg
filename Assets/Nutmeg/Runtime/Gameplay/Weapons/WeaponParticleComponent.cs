using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Nutmeg.Runtime.Gameplay.Weapons
{
    public class WeaponParticleComponent : WeaponComponent
    {
        [SerializeField] private int maxPoolSize;
        [SerializeField] private GameObject particle;

        private List<object> pool;
        
        
        public override bool Get(out object data)
        {
            throw new System.NotImplementedException();
        }

        
    }
}