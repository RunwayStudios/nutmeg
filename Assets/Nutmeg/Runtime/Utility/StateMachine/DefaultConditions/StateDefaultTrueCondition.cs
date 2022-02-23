using UnityEngine;

namespace Nutmeg.Runtime.Utility.StateMachine.DefaultConditions
{
    [CreateAssetMenu(fileName = "defaultTrue", menuName = "StateMachine/Conditions/Default True")]
    internal class StateDefaultTrueCondition : StateCondition
    {
        public override void Initialize(StateMachine root){}

        public override bool IsMet() => true;
    }
}
