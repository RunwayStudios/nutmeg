using System;
using System.Collections.Generic;
using Nutmeg.Runtime.Utility.InputSystem;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Nutmeg.Editor.Utility.InputSystem
{
    [CustomEditor(typeof(InputState))]
    public class UIStateEditor : UnityEditor.Editor
    {
        private InputState inputState;
        private InputActions inputActions;

        private bool[] actionMapsFoldouts;
        

        private void OnEnable()
        {
            inputState = (InputState)target;
            inputActions = InputManager.Input ?? new InputActions();

            IEnumerator<InputAction> iaIterator = inputActions.GetEnumerator();
            int actionMapCount = 0;
            Guid prevActionMap = Guid.Empty;
            List<Guid> activeInputActions = new List<Guid>();
            while (iaIterator.MoveNext())
            {
                Guid actionMapID = iaIterator.Current.actionMap.id;
                Guid curID = iaIterator.Current.id;
                
                if (prevActionMap != actionMapID)
                {
                    prevActionMap = actionMapID;
                    actionMapCount++;
                }

                if (!inputState.inputs.ContainsKey(curID))
                    inputState.inputs.Add(curID, false);
                
                if (!inputState.inputIgnores.ContainsKey(curID))
                    inputState.inputIgnores.Add(curID, true);

                activeInputActions.Add(curID);
            }

            actionMapsFoldouts = new bool[actionMapCount];
            iaIterator.Dispose();


            Dictionary<Guid, bool>.Enumerator iasToCheck = inputState.inputs.GetEnumerator();
            List<Guid> iasToRemove = new List<Guid>();
            while (iasToCheck.MoveNext())
            {
                if (!activeInputActions.Contains(iasToCheck.Current.Key))
                    iasToRemove.Add(iasToCheck.Current.Key);
            }
            iasToCheck.Dispose();
            
            for (int i = 0; i < iasToRemove.Count; i++)
                inputState.inputs.Remove(iasToRemove[i]);
            
            
            Dictionary<Guid, bool>.Enumerator iisToCheck = inputState.inputIgnores.GetEnumerator();
            List<Guid> iisToRemove = new List<Guid>();
            while (iisToCheck.MoveNext())
            {
                if (!activeInputActions.Contains(iisToCheck.Current.Key))
                    iisToRemove.Add(iisToCheck.Current.Key);
            }
            iisToCheck.Dispose();
            
            for (int i = 0; i < iisToRemove.Count; i++)
                inputState.inputIgnores.Remove(iisToRemove[i]);
        }

        public override void OnInspectorGUI()
        {
            IEnumerator<InputAction> iaIterator = inputActions.GetEnumerator();
            int actionMapIndex = -1;
            int inputActionIndex = 0;
            Guid prevActionMap = Guid.Empty;
            while (iaIterator.MoveNext())
            {
                Guid actionMapID = iaIterator.Current.actionMap.id;
                Guid curID = iaIterator.Current.id;
                
                if (prevActionMap != actionMapID)
                {
                    prevActionMap = actionMapID;
                    actionMapIndex++;

                    ToggleGroupState actionMapToggles = CheckAllActionMapToggles(inputActionIndex, actionMapID);
                    bool toggleTo = true;
                    bool curActionMapToggleDisplayValue = false;
                    if (actionMapToggles == ToggleGroupState.AllTrue)
                    {
                        toggleTo = false;
                        curActionMapToggleDisplayValue = true;
                    }
                    ToggleGroupState actionMapIgnoreToggles = CheckAllActionMapIgnoreToggles(inputActionIndex, actionMapID);
                    bool toggleIgnoresTo = true;
                    bool curActionMapToggleIgnoreDisplayValue = false;
                    if (actionMapIgnoreToggles == ToggleGroupState.AllTrue)
                    {
                        toggleIgnoresTo = false;
                        curActionMapToggleIgnoreDisplayValue = true;
                    }

                    Rect r = EditorGUILayout.BeginHorizontal();
                    
                    actionMapsFoldouts[actionMapIndex] = EditorGUILayout.Foldout(actionMapsFoldouts[actionMapIndex], GUIContent.none);
                    r.x += 5;
                    
                    EditorGUI.BeginChangeCheck();
                    EditorGUI.showMixedValue = actionMapToggles == ToggleGroupState.Mixed;
                    r.width /= 3;
                    EditorGUI.Toggle(r, curActionMapToggleDisplayValue);
                    EditorGUI.showMixedValue = false;
                    if (EditorGUI.EndChangeCheck())
                    {
                        ToggleAll(inputActionIndex, actionMapID, toggleTo);
                    }
                    
                    r.x += 15;
                    GUIStyle guiStyleBold = new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold };
                    GUI.Label(r, iaIterator.Current.actionMap.name, guiStyleBold);

                    EditorGUI.BeginChangeCheck();
                    EditorGUI.showMixedValue = actionMapIgnoreToggles == ToggleGroupState.Mixed;
                    EditorGUILayout.ToggleLeft("ignore all", curActionMapToggleIgnoreDisplayValue, GUILayout.ExpandWidth(false));
                    EditorGUI.showMixedValue = false;
                    if (EditorGUI.EndChangeCheck())
                    {
                        ToggleAllIgnores(inputActionIndex, actionMapID, toggleIgnoresTo);
                    }

                    EditorGUILayout.EndHorizontal();
                }

                if (actionMapsFoldouts[actionMapIndex])
                {
                    EditorGUILayout.BeginHorizontal();
                    inputState.inputs[curID] = EditorGUILayout.ToggleLeft(iaIterator.Current.name, inputState.inputs[curID]);
                    inputState.inputIgnores[curID] = EditorGUILayout.ToggleLeft("ignore", inputState.inputIgnores[curID]);
                    EditorGUILayout.EndHorizontal();
                }

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
                    first = inputState.inputs[iaIterator.Current.id];
                    continue;
                }

                if (actionMapID != iaIterator.Current.actionMap.id)
                {
                    iaIterator.Dispose();
                    return first ? ToggleGroupState.AllTrue : ToggleGroupState.AllFalse;
                }

                if (first == inputState.inputs[iaIterator.Current.id])
                    continue;

                iaIterator.Dispose();
                return ToggleGroupState.Mixed;
            }

            iaIterator.Dispose();
            return first ? ToggleGroupState.AllTrue : ToggleGroupState.AllFalse;
        }

        private ToggleGroupState CheckAllActionMapIgnoreToggles(int startIndex, Guid actionMapID)
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
                    first = inputState.inputIgnores[iaIterator.Current.id];
                    continue;
                }

                if (actionMapID != iaIterator.Current.actionMap.id)
                {
                    iaIterator.Dispose();
                    return first ? ToggleGroupState.AllTrue : ToggleGroupState.AllFalse;
                }

                if (first == inputState.inputIgnores[iaIterator.Current.id])
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

                inputState.inputs[iaIterator.Current.id] = value;
            }

            iaIterator.Dispose();
        }
        
        private void ToggleAllIgnores(int startIndex, Guid actionMapID, bool value)
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

                inputState.inputIgnores[iaIterator.Current.id] = value;
            }

            iaIterator.Dispose();
        }
    }
}