using UnityEngine;

namespace Nutmeg.Runtime.Gameplay.Items
{
    [CreateAssetMenu(fileName = "new ItemMeta", menuName = "item/itemMeta")]
    public class ItemMeta : ScriptableObject
    {
        public string name;
        public string description;
        public ItemType type;
    }
}