using Runway.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "defaultFalse", menuName = "StateMachine/Conditions/Default False")]
public class StateDefaultFalseCondition : StateCondition
{
    public override void Initialize(StateMachine root){}

    public override bool IsMet() => false;
}
