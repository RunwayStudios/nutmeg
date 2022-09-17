using Nutmeg.Runtime.Utility.StateMachine;
using UnityEngine;

namespace Nutmeg.Runtime.Gameplay.Player.StateConditions
{
    [CreateAssetMenu(fileName = "IsDashingCondition", menuName = "StateMachine/Conditions/Is Dashing")]
    public class IsDashingCondition : StateCondition
    {
        private PlayerController controller;

        public override bool IsMet()
        {
            return controller.IsDashing;
        }

        public override void Initialize(StateMachine root)
        {
            controller = root.GetComponent<PlayerController>();
        }
    }
}