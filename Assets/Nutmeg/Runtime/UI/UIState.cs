using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Nutmeg.Runtime.UI
{
    [CreateAssetMenu(menuName = "UI/UIState")]
    public class UIState : ScriptableObject
    {
        public Dictionary<Guid, bool> inputs = new Dictionary<Guid, bool>();

        
    }
}
