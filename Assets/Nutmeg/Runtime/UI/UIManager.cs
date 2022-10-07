using System;
using System.Collections.Generic;
using Nutmeg.Runtime.Utility.InputSystem;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Nutmeg.Runtime.UI
{
    public class UIManager : MonoBehaviour
    {
        private InputActions input;
    
        // Start is called before the first frame update
        void Start()
        {
            input = InputManager.Input;
        
            IEnumerator<InputAction> inputActions = input.GetEnumerator();

            while (inputActions.MoveNext())
            {
                Debug.Log(inputActions.Current.id);
                // inputActions.Current.Disable();
            }
        
            inputActions.Dispose();
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
