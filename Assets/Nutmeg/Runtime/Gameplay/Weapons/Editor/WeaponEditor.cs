using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;

namespace Nutmeg.Runtime.Gameplay.Weapons.Editor
{
    [CustomEditor(typeof(Weapon))]
    public class WeaponEditor : UnityEditor.Editor
    {
        private Weapon root;
        private SerializedProperty presetProperty;
        private WeaponPreset preset;

        private Dictionary<WeaponPreset, Type[]> presets = new()
        {
            {
                WeaponPreset.Rifle,
                new[]
                {
                    typeof(WeaponHitScanComponent), typeof(WeaponAmmunitionComponent), typeof(WeaponOriginComponent)
                }
            },
            {WeaponPreset.RocketLauncher, new[] {typeof(WeaponAmmunitionComponent)}}
        };

        private void OnEnable()
        {
            root = (Weapon) target;
            presetProperty = serializedObject.FindProperty("preset");
        }

        public override void OnInspectorGUI()
        {
            UpdateComponents();

            EditorGUI.BeginChangeCheck();
            preset = (WeaponPreset) presetProperty.enumValueFlag;
            EditorGUILayout.PropertyField(presetProperty, new GUIContent("Weapon preset"));
            serializedObject.ApplyModifiedProperties();
            if (EditorGUI.EndChangeCheck())
            {
                try
                {
                    RemoveComponents(presets[preset]);
                }
                catch (Exception)
                {
                    // ignored
                }

                preset = (WeaponPreset) presetProperty.enumValueFlag;
                try
                {
                    AddComponents(presets[preset]);
                }
                catch (Exception)
                {
                    // ignored
                }
            }

            base.OnInspectorGUI();
        }

        private void AddComponents(Type[] types)
        {
            foreach (var t in types)
                root.gameObject.AddComponent(t);
        }

        private void RemoveComponents(Type[] types)
        {
            foreach (var t in types)
                DestroyImmediate(root.gameObject.GetComponent(t));
        }

        private void UpdateComponents()
        {
            Type[] targetComponents = root.GetComponents<WeaponComponent>().Select(c => c.GetType()).ToArray();
            Type[] presetComponents = Type.EmptyTypes;
            
            try
            {
                presetComponents = presets[root.preset];
            }
            catch (Exception)
            {
                //ignored
            }
            
            List<Type> excessiveComponents = targetComponents.ToList();
            List<Type> missingComponents = presetComponents.ToList();

            foreach (var component in presetComponents)
            {
                if (excessiveComponents.Contains(component))
                    excessiveComponents.Remove(component);
            }
            RemoveComponents(excessiveComponents.ToArray());

            foreach (var component in targetComponents)
            {
                if (missingComponents.Contains(component))
                    missingComponents.Remove(component);
            }
            AddComponents(missingComponents.ToArray());
        }
    }
}