using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Runway.Core;

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
