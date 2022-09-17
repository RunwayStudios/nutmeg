using UnityEngine;

namespace Nutmeg.Runtime.Gameplay.Player.StateConditions
{
    [CreateAssetMenu(fileName = "IsNotDashingCondition", menuName = "StateMachine/Conditions/Is not Dashing")]
    public class IsNotDashingCondition : IsDashingCondition
    {
        public override bool IsMet()
        {
            return !base.IsMet();
        }
    }
}