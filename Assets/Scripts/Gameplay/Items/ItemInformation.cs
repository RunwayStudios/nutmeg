using UnityEngine;

namespace UnityTemplateProjects.Gameplay.Items
{
    [CreateAssetMenu(fileName = "new ItemInfo", menuName = "MENUNAME", order = 0)]
    public class ItemInformation : ScriptableObject
    {
        public string name;
    }
}