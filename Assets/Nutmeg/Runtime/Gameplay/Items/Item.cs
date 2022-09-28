using System;
using System.Collections;
using JetBrains.Annotations;
using UnityEngine;

namespace Nutmeg.Runtime.Gameplay.Items
{
    public abstract class Item : MonoBehaviour
    {
        public abstract void Use();

        private void Initialize()
        {
            
        } 
    }
}