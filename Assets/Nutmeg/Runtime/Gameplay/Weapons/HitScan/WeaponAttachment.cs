using Nutmeg.Runtime.Gameplay.Items;
using UnityEngine;

namespace Nutmeg.Runtime.Gameplay.Weapons.HitScan
{
    [CreateAssetMenu(fileName = "new WeaponAttachment", menuName = "Item/Weapon/Attachment")]
    public class WeaponAttachment : ItemDataObject
    {
        public AttachmentType type;
    }

    public enum AttachmentType
    {
        none,
        barrel,
        body,
        handle,
        magazine,
        muzzle,
        optic,
        stock,
        underbarrel
    } 
}