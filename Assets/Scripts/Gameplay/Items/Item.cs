using UnityEngine;

namespace Gameplay.Items
{
    public abstract class Item : MonoBehaviour
    {
        [SerializeField] private ItemData itemInfo;

        public ItemType type;
    
        public abstract void Use();
    }
    
    public enum ItemType
    {
        weapon
    }
}
