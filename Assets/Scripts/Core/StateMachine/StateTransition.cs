using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace Runway.Core
{

    [CreateAssetMenu(fileName = "StateTransition", menuName = "StateMachine/Transition")]
    public class StateTransition : ScriptableObject
    {
        [SerializeField] private ConditionContainer[] conditions;
        [SerializeField] public State transitionState;

        private void OnValidate()
        {
            if (conditions.Length > 0)
                conditions[conditions.Length - 1].lastItem = true;
        }

        public bool Transit(StateMachine stateMachine)
        {
            foreach (ConditionContainer container in conditions)
            {
                StateCondition condition = container.condition;
                condition.Initialize(stateMachine);

                if (container.operatorSelected == 0 && condition.IsMet()) return true;

                if (container.operatorSelected != 0 && !condition.IsMet()) return false;
            }
            return false;
        }

        //[System.Serializable]
        //public class ConditionContainer
        //{
        //    public StateCondition condition;
        //    [HideInInspector] public bool lastItem = false;

        //    public enum @operator : int
        //    {
        //        OR = 0,
        //        AND = 1
        //    };
        //    public @operator operatorSelected = @operator.OR;
        //}

        //#if UNITY_EDITOR
        //[CustomEditor(typeof(ConditionContainer))]
        //class ConditionEditor : Editor
        //{
        //    SerializedProperty enumProp;

        //    private void OnEnable()
        //    {
        //        enumProp = serializedObject.FindProperty("operatorSelected");
        //    }

        //    public override void OnInspectorGUI()
        //    {
        //        //base.OnInspectorGUI();

        //        serializedObject.Update();
        //        GUILayout.Label("test");
        //        EditorGUILayout.PropertyField(enumProp);

        //        serializedObject.ApplyModifiedProperties();
        //    }
        //}
        //#endif
    }
}