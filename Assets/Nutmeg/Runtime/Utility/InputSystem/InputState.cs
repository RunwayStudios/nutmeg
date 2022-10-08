using System;
using System.Collections.Generic;
using UnityEngine;

namespace Nutmeg.Runtime.Utility.InputSystem
{
    [CreateAssetMenu(menuName = "Input/InputState")]
    public class InputState : ScriptableObject
    {
        public Dictionary<Guid, bool> inputs = new Dictionary<Guid, bool>();
        public Dictionary<Guid, bool> inputIgnores = new Dictionary<Guid, bool>();
    }
}
