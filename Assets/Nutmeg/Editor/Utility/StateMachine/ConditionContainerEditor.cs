using System.Collections;
using System.Collections.Generic;
using Nutmeg.Runtime.Utility.StateMachine;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ConditionContainer))]
public class ConditionContainerEditor : Editor
{
    SerializedProperty enumProp;

    private void OnEnable()
    {
        enumProp = serializedObject.FindProperty("operatorSelected");
    }

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();

        serializedObject.Update();
        GUILayout.Label("test");
        EditorGUILayout.PropertyField(enumProp);

        serializedObject.ApplyModifiedProperties();
    }

}
