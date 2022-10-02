using System;
using Unity.Netcode;
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

        public void Get() => Get(out _);

        public T Get<T>()
        {
            if (Get(out var o))
                return (T) o;
            return default;
        }
    }
}