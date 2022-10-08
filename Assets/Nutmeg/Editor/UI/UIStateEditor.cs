using System;
using System.Collections.Generic;
using Nutmeg.Runtime.UI;
using Nutmeg.Runtime.Utility.InputSystem;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Nutmeg.Editor.UI
{
    [CustomEditor(typeof(UIState))]
    public class UIStateEditor : UnityEditor.Editor
    {
        private UIState uiState;
        private InputActions inputActions;

        private bool[] actionMapsFoldouts;

        private void OnEnable()
        {
            uiState = (UIState)target;
            inputActions = InputManager.Input ?? new InputActions();

            IEnumerator<InputAction> iaIterator = inputActions.GetEnumerator();
            int actionMapCount = 0;
            Guid prevActionMap = Guid.Empty;
            List<Guid> activeInputActions = new List<Guid>();
            while (iaIterator.MoveNext())
            {
                if (prevActionMap != iaIterator.Current.actionMap.id)
                {
                    prevActionMap = iaIterator.Current.actionMap.id;
                    actionMapCount++;
                }

                if (!uiState.inputs.ContainsKey(iaIterator.Current.id))
                    uiState.inputs.Add(iaIterator.Current.id, false);

                activeInputActions.Add(iaIterator.Current.id);
            }

            actionMapsFoldouts = new bool[actionMapCount];
            iaIterator.Dispose();


            Dictionary<Guid, bool>.Enumerator iasToCheck = uiState.inputs.GetEnumerator();
            List<Guid> iasToRemove = new List<Guid>();
            while (iasToCheck.MoveNext())
            {
                if (!activeInputActions.Contains(iasToCheck.Current.Key))
                {
                    iasToRemove.Add(iasToCheck.Current.Key);
                }
            }

            iasToCheck.Dispose();

            for (int i = 0; i < iasToRemove.Count; i++)
                uiState.inputs.Remove(iasToRemove[i]);
        }

        public override void OnInspectorGUI()
        {
            IEnumerator<InputAction> iaIterator = inputActions.GetEnumerator();
            int actionMapIndex = -1;
            int inputActionIndex = 0;
            Guid prevActionMap = Guid.Empty;
            while (iaIterator.MoveNext())
            {
                if (prevActionMap != iaIterator.Current.actionMap.id)
                {
                    prevActionMap = iaIterator.Current.actionMap.id;
                    actionMapIndex++;

                    ToggleGroupState actionMapToggles = CheckAllActionMapToggles(inputActionIndex, iaIterator.Current.actionMap.id);
                    bool toggleTo = false;
                    bool curActionMapToggleDisplayValue = false;
                    switch (actionMapToggles)
                    {
                        case ToggleGroupState.AllTrue:
                            curActionMapToggleDisplayValue = true;
                            break;
                        case ToggleGroupState.AllFalse:
                            toggleTo = true;
                            break;
                        case ToggleGroupState.Mixed:
                            toggleTo = true;
                            break;
                    }

                    Rect r = EditorGUILayout.BeginHorizontal();
                    
                    actionMapsFoldouts[actionMapIndex] = EditorGUILayout.Foldout(actionMapsFoldouts[actionMapIndex], GUIContent.none);
                    r.x += 5;
                    
                    EditorGUI.BeginChangeCheck();
                    GUI.Toggle(r, curActionMapToggleDisplayValue, GUIContent.none);
                    if (EditorGUI.EndChangeCheck())
                    {
                        ToggleAll(inputActionIndex, iaIterator.Current.actionMap.id, toggleTo);
                    }
                    
                    if (actionMapToggles == ToggleGroupState.Mixed)
                    {
                        r.y -= 1;
                        GUIStyle guiStyle = new GUIStyle(GUI.skin.label)
                        {
                            // default 12
                            fontSize = 20,
                            fontStyle = FontStyle.Bold
                        };
                        GUI.Label(r, "-", guiStyle);
                        r.y += 1;
                    }
                    r.x += 15;
                    GUI.Label(r, iaIterator.Current.actionMap.name);

                    EditorGUILayout.EndHorizontal();
                }

                if (actionMapsFoldouts[actionMapIndex])
                    uiState.inputs[iaIterator.Current.id] = EditorGUILayout.ToggleLeft(iaIterator.Current.name, uiState.inputs[iaIterator.Current.id]);

                inputActionIndex++;
            }

            iaIterator.Dispose();
        }


        private enum ToggleGroupState
        {
            AllTrue,
            AllFalse,
            Mixed
        }
        
        private ToggleGroupState CheckAllActionMapToggles(int startIndex, Guid actionMapID)
        {
            IEnumerator<InputAction> iaIterator = inputActions.GetEnumerator();
            int inputActionIndex = -1;
            bool first = false;
            while (iaIterator.MoveNext())
            {
                inputActionIndex++;
                if (inputActionIndex < startIndex)
                    continue;

                if (inputActionIndex == startIndex)
                {
                    first = uiState.inputs[iaIterator.Current.id];
                    continue;
                }

                if (actionMapID != iaIterator.Current.actionMap.id)
                {
                    iaIterator.Dispose();
                    return first ? ToggleGroupState.AllTrue : ToggleGroupState.AllFalse;
                }

                if (first == uiState.inputs[iaIterator.Current.id])
                    continue;

                iaIterator.Dispose();
                return ToggleGroupState.Mixed;
            }

            iaIterator.Dispose();
            return first ? ToggleGroupState.AllTrue : ToggleGroupState.AllFalse;
        }

        private void ToggleAll(int startIndex, Guid actionMapID, bool value)
        {
            IEnumerator<InputAction> iaIterator = inputActions.GetEnumerator();
            int inputActionIndex = -1;
            while (iaIterator.MoveNext())
            {
                inputActionIndex++;
                if (inputActionIndex < startIndex)
                    continue;

                if (actionMapID != iaIterator.Current.actionMap.id)
                {
                    iaIterator.Dispose();
                    return;
                }

                uiState.inputs[iaIterator.Current.id] = value;
            }

            iaIterator.Dispose();
        }
    }
}