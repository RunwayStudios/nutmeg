using UnityEngine;
using UnityTemplateProjects.Gameplay;

namespace Gameplay.Items
{
    [CreateAssetMenu(fileName = "new Stats", menuName = "Stats")]
    public class Stats : ScriptableObject
    {
        [SerializeField] private StatObject[] stats;

        public float this[StatType type] => GetValueOfType(type);

        public float GetValueOfType(StatType type) => TryGetValueOfType(type, out float f) ? f : 0f;

        public bool TryGetValueOfType(StatType type, out float value)
        {
            foreach (StatObject s in stats)
            {
                if (s.type == type)
                {
                    value = s.value;
                    return true;
                }
            }

            value = 0f;
            return false;
        }
    }
    
    [System.Serializable]
    public class StatObject
    {
        public StatType type;
        public float value;
    }
}