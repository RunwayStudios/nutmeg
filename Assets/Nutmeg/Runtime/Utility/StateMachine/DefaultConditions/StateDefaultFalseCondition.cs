using UnityEngine;

namespace Nutmeg.Runtime.Utility.StateMachine.DefaultConditions
{
    [CreateAssetMenu(fileName = "defaultFalse", menuName = "StateMachine/Conditions/Default False")]
    internal class StateDefaultFalseCondition : StateCondition
    {
        public override void Initialize(StateMachine root){}

        public override bool IsMet() => false;
    }
}
