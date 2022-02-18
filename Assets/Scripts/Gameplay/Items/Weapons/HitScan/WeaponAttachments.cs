using UnityEngine;

namespace Gameplay.Items.Weapons
{
    [CreateAssetMenu(fileName = "new WeaponAttachment", menuName = "Item/Weapon/Attachment")]
    public class WeaponAttachments : ItemData
    {
        public AttachmentType type;
        public WeaponStats statDifference;
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