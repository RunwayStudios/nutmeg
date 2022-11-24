using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Nutmeg.Runtime.Utility.InputSystem
{
    [CreateAssetMenu(menuName = "Input/InputState")]
    public class InputState : ScriptableObject
    {
        public Dictionary<Guid, bool> inputs;
        public Dictionary<Guid, bool> inputIgnores;

        [SerializeField] private List<string> serializedGuids;
        [SerializeField] private List<bool> serializedInputs;
        [SerializeField] private List<bool> serializedIgnores;
        

        private void OnEnable()
        {
            inputs ??= new Dictionary<Guid, bool>();
            inputIgnores ??= new Dictionary<Guid, bool>();
            
            // Debug.Log("OnEnable so guids" + serializedGuids.Count + "\n" +
            //           "OnEnable so inputs" + serializedInputs.Count + "\n" +
            //           "OnEnable so ignores" + serializedIgnores.Count);
            
            
            for (int i = 0; i < serializedGuids.Count; i++)
            {
                inputs.Add(Guid.Parse(serializedGuids[i]), serializedInputs[i]);
                inputIgnores.Add(Guid.Parse(serializedGuids[i]), serializedIgnores[i]);
            }

            // Debug.Log("OnEnable so" + inputs.Count);
        }

        // private void OnDisable()
        // {
        //     // Debug.Log("OnDisable so" + inputs.Count + "\n" +
        //     //           "OnDisable so guids" + serializedGuids.Count + "\n" +
        //     //           "OnDisable so inputs" + serializedInputs.Count + "\n" +
        //     //           "OnDisable so ignores" + serializedIgnores.Count);
        //     
        //     Deserialize();
        // }

        public void Deserialize()
        {
            // Debug.Log("Deserialize" + inputs.Count + "\n" +
            //           "Deserialize guids" + serializedGuids.Count + "\n" +
            //           "Deserialize inputs" + serializedInputs.Count + "\n" +
            //           "Deserialize ignores" + serializedIgnores.Count);

            serializedGuids.Clear();
            serializedInputs.Clear();
            serializedIgnores.Clear();
            
            Dictionary<Guid, bool>.Enumerator iterator = inputs.GetEnumerator();
            while (iterator.MoveNext())
            {
                serializedGuids.Add(iterator.Current.Key.ToString());
                serializedInputs.Add(iterator.Current.Value);
            }
            
            iterator.Dispose();
            
            iterator = inputIgnores.GetEnumerator();
            while (iterator.MoveNext())
            {
                serializedIgnores.Add(iterator.Current.Value);
            }
            
            iterator.Dispose();

#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
        }
    }
}