using Nutmeg.Runtime.Gameplay.Combat.CombatModules;
using UnityEngine;

namespace Nutmeg.Runtime.Gameplay.Weapons
{
    [CreateAssetMenu(fileName = "new WeaponStats", menuName = "Weapon/Stats")]
    public class WeaponStats : ScriptableObject
    {
        public DamageType damageType;
        public float fireRate;
        public float reloadTime;
        public float damage;
        public float range;
        public int maxAmmunition;
        public float accuracy;
    }
}