using UnityEngine;

namespace Gameplay.Items
{
    public abstract class Item : MonoBehaviour
    {
        [SerializeField] private ItemDataObject itemDataObject;
        [SerializeField] private Stats stats;

        public Stats Stats => stats != null ? stats : ScriptableObject.CreateInstance<Stats>();
        
        public abstract void Use();
    }
}
