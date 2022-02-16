using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Runway.Core
{
    [CreateAssetMenu(fileName = "State", menuName = "StateMachine/State")]
    public class State : ScriptableObject
    {
        public PlayerState playerState;
        [SerializeField] private StateTransition[] transitions;

        public bool OnTransition(out State newState, StateMachine stateMachine)
        {
            foreach (StateTransition transition in transitions)
            {
                if (transition.Transit(stateMachine))
                {
                    //Debug.Log(stateName + " -> " + transition.transitionState);
                    newState = transition.transitionState;
                    return true;
                }
            }

            newState = null;
            return false;
        }

        public override string ToString()
        {
            return playerState.ToString();
        }
    }
}
