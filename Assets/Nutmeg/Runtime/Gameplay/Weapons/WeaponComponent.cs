using System;
using UnityEngine;

namespace Nutmeg.Runtime.Gameplay.Weapons
{
    public abstract class WeaponComponent : MonoBehaviour
    {
        protected Weapon root;

        protected virtual void Start()
        {
            root = GetComponent<Weapon>();
        }

        public abstract bool Get(out object data);

    }
}