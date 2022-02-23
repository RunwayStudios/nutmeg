using UnityEngine;

namespace Nutmeg.Runtime.Utility.StateMachine
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