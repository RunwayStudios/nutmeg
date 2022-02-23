using UnityEngine;

namespace Nutmeg.Runtime.Utility.StateMachine
{
    public abstract class StateCondition : ScriptableObject
    {
        /// <summary>
        /// Determins if state of the condition. Is being called after the initialization of the condition
        /// </summary>
        /// <returns> If the condition is met</returns>
        public abstract bool IsMet();

        /// <summary>
        /// Gets called before the condition check is being done
        /// </summary>
        /// <param name="root"> The refrence of the root state machine</param>
        public abstract void Initialize(StateMachine root);
    }
}
