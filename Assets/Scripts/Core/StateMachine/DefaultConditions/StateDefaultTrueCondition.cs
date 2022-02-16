using Runway.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "defaultTrue", menuName = "StateMachine/Conditions/Default True")]
public class StateDefaultTrueCondition : StateCondition
{
    public override void Initialize(StateMachine root){}

    public override bool IsMet() => true;
}
