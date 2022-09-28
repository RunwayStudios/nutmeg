using UnityEngine;

namespace Nutmeg.Runtime.Gameplay.Weapons
{
    public abstract class WeaponDamageComponent : WeaponComponent
    {
        protected override void Start()
        {
            base.Start();
            root.hitComponent = this;
        }
    }
}