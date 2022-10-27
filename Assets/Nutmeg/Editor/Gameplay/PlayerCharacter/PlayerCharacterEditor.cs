using System;
using Nutmeg.Runtime.Gameplay.PlayerCharacter;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlayerCharacter))]
public class PlayerCharacterEditor : Editor
{
    private PlayerCharacter character;
    private Editor gameObjectEditor;

    private void OnEnable()
    {
        character = (PlayerCharacter) target;
    }

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();
        base.OnInspectorGUI();

        //GameObject gameObject = (GameObject) EditorGUILayout.ObjectField(character.prefab, typeof(GameObject), true);

        if (character.prefab != null && gameObjectEditor == null || EditorGUI.EndChangeCheck())
            gameObjectEditor = CreateEditor(character.prefab);

        gameObjectEditor.OnPreviewGUI(GUILayoutUtility.GetRect(250, 250), EditorStyles.whiteLabel);
    }
}