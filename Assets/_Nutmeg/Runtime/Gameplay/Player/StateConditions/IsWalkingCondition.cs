using Runway.Core;
using UnityEngine;

[CreateAssetMenu(fileName = "IsWalkingCondition", menuName = "StateMachine/Conditions/Is Walking", order = 0)]
public class IsWalkingCondition : StateCondition
{
    private PlayerController controller;

    public override bool IsMet()
    {
        return controller.IsWalking;
    }

    public override void Initialize(StateMachine root)
    {
        controller = root.GetComponent<PlayerController>();
    }
}