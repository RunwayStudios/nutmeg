using Nutmeg.Runtime.Gameplay.Combat.CombatModules;

namespace Nutmeg.Runtime.Gameplay.Weapons
{
    public abstract class WeaponDamageOverrideComponent : WeaponComponent
    {
        public DamageableModule[] modules;
    }
}