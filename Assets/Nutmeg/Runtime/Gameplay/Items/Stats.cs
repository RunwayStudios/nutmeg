﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Nutmeg.Runtime.Gameplay.Items
{
    [CreateAssetMenu(fileName = "new Stats", menuName = "Stats")]
    public class Stats : ScriptableObject
    {
        [SerializeField] private StatObject[] stats;

        private Dictionary<StatType, float> statMap;

        public float this[StatType type] => GetValueOfType(type);

        public bool HasType(StatType type) => statMap.ContainsKey(type);

        public float GetValueOfType(StatType type) => TryGetValueOfType(type, out float f) ? f : 0f;

        public bool TryGetValueOfType(StatType type, out float value)
        {
            if (statMap.TryGetValue(type, out float f))
            {
                value = f;
                return true;
            }

            Debug.LogWarning(
                $"You are trying to access the stat type {type} of {name} but {name} doesn't have a {type} stat type");
            value = 0f;
            return false;
        }

        private void OnValidate()
        {
            statMap = stats.ToDictionary(s => s.type, s => s.value);
        }
    }

    [System.Serializable]
    public class StatObject
    {
        public StatType type;
        public float value;
    }
}