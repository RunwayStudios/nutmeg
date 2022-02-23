using System;
using UnityEngine;

namespace Runway.Core
{
    public class StateMachine : MonoBehaviour
    {
        [SerializeField] private State initialState;
        [SerializeField] private bool logTransition;
        public bool updateStateOnUpdate = true;
        public PlayerState State => currentState.playerState;

        //public PlayerState State => currentState.playerState;
        public Action<PlayerState> onStateEnter;
        public Action<PlayerState> onStateExit;

        private State currentState;

        private void OnEnable()
        {
            currentState = initialState;
        }

        private void Update()
        {
            if (updateStateOnUpdate)
                UpdateState();
        }

        public void UpdateState()
        {
            if (currentState.OnTransition(out State newState, this))
            {
                if (logTransition)
                    Debug.Log($"{currentState} => {newState}");

                State oldState = currentState;
                currentState = newState;
                onStateExit?.Invoke(oldState.playerState);
                onStateEnter?.Invoke(currentState.playerState);
            }
        }
    }
}