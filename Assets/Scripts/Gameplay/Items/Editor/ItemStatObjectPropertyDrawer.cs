using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ItemStatObjectPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect r, SerializedProperty p, GUIContent l)
    {
        Rect typeRect = new Rect(r.x, r.y, r.width, 21f);
        Rect valueRect = new Rect(r.x, r.y + 21f, r.width, 21f);
        Rect damageTypeRect = new Rect(r.x, r.y + 42f, r.width, 21f);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return base.GetPropertyHeight(property, label) + 42f;
    }
}
