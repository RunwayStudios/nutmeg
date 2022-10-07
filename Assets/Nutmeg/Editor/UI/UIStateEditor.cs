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

        private bool[] actionMaps;

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

            actionMaps = new bool[actionMapCount];
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
            Guid prevActionMap = Guid.Empty;
            while (iaIterator.MoveNext())
            {
                if (prevActionMap != iaIterator.Current.actionMap.id)
                {
                    prevActionMap = iaIterator.Current.actionMap.id;
                    actionMapIndex++;

                    Rect r = EditorGUILayout.BeginHorizontal("Button");
                    actionMaps[actionMapIndex] = EditorGUILayout.Foldout(actionMaps[actionMapIndex], iaIterator.Current.actionMap.name);
                    if (actionMaps[actionMapIndex])
                    {
                        Rect buttonRect = r;
                        buttonRect.width /= 2;
                        buttonRect.x += buttonRect.width / 2;
                        if (GUI.Button(buttonRect, "toggle all"))
                        {
                            ToggleAll(actionMapIndex, iaIterator.Current.actionMap.id);
                        }
                    }

                    EditorGUILayout.EndHorizontal();
                }

                if (actionMaps[actionMapIndex])
                    uiState.inputs[iaIterator.Current.id] = EditorGUILayout.ToggleLeft(iaIterator.Current.name, uiState.inputs[iaIterator.Current.id]);
            }

            iaIterator.Dispose();
        }


        private void ToggleAll(int startIndex, Guid id)
        {
            IEnumerator<InputAction> iaIterator = inputActions.GetEnumerator();
            int actionMapIndex = -1;
            bool first = false;
            while (iaIterator.MoveNext())
            {
                actionMapIndex++;
                if (actionMapIndex < startIndex)
                    continue;

                if (actionMapIndex == startIndex)
                {
                    first = actionMaps[actionMapIndex];
                    continue;
                }

                if (id != iaIterator.Current.actionMap.id)
                {
                    iaIterator.Dispose();
                    ToggleAll(startIndex, id, !first);
                    return;
                }

                if (first == actionMaps[actionMapIndex])
                    continue;
                
                iaIterator.Dispose();
                ToggleAll(startIndex, id, false);
                return;
            }

            iaIterator.Dispose();
            ToggleAll(startIndex, id, !first);
        }
        
        private void ToggleAll(int startIndex, Guid id,  bool value)
        {
            IEnumerator<InputAction> iaIterator = inputActions.GetEnumerator();
            int actionMapIndex = -1;
            while (iaIterator.MoveNext())
            {
                actionMapIndex++;
                if (actionMapIndex < startIndex)
                    continue;

                if (id != iaIterator.Current.actionMap.id)
                {
                    iaIterator.Dispose();
                    return;
                }

                actionMaps[actionMapIndex] = value;
            }

            iaIterator.Dispose();
        }
    }
}