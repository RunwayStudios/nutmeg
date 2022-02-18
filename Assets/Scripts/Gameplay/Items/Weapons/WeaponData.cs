using Gameplay.Items.Weapons;
using UnityEngine;

namespace Gameplay.Items
{
    [CreateAssetMenu(fileName = "WeaponData", menuName = "Item/Weapon/Data")]
    public class WeaponData : ItemData
    {
        public WeaponStats stats;

        public WeaponAttachments[] attachments;
    }
}