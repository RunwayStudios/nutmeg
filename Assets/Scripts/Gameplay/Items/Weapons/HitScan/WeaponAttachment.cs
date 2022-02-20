using UnityEngine;

namespace Gameplay.Items.Weapons
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