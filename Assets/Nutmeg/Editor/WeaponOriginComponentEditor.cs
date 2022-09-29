using System;
using Nutmeg.Runtime.Gameplay.Weapons;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WeaponOriginComponent))]
public class WeaponOriginComponentEditor : Editor
{
    private WeaponOriginComponent root;

    private SerializedProperty singleOriginProperty;
    private SerializedProperty multipleOriginsProperty;
    private SerializedProperty multipleOriginsTypeProperty;

    private void OnEnable()
    {
        root = (WeaponOriginComponent) target;

        singleOriginProperty = serializedObject.FindProperty("singleOrigin");
        multipleOriginsProperty = serializedObject.FindProperty("multipleOrigins");
        multipleOriginsTypeProperty = serializedObject.FindProperty("multipleOriginsType");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (root.originType == OriginType.Single)
        {
            EditorGUILayout.PropertyField(singleOriginProperty, new GUIContent("Origin"));
        }
        else if (root.originType == OriginType.Multiple)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(multipleOriginsTypeProperty, new GUIContent("Origins access type"));
            if (EditorGUI.EndChangeCheck())
                root.ResetOriginIndex();
            EditorGUILayout.PropertyField(multipleOriginsProperty, new GUIContent("Origins"));
        }

        serializedObject.ApplyModifiedProperties();
    }
}