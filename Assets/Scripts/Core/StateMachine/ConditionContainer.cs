using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Runway.Core
{
    [System.Serializable]
    public class ConditionContainer
    {
        public StateCondition condition;
        [HideInInspector] public bool lastItem = false;

        public enum @operator : int
        {
            OR = 0,
            AND = 1
        };
        public @operator operatorSelected = @operator.OR;
    }
}