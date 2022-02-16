using UnityEngine;

namespace UnityTemplateProjects.Gameplay.Player.StateConditions
{
    [CreateAssetMenu(fileName = "IsNotWalkingCondition", menuName = "StateMachine/Conditions/Is not Walking")]
    public class IsNotWalkingCondition : IsWalkingCondition
    {
        public override bool IsMet()
        {
            return !base.IsMet();
        }
    }
}