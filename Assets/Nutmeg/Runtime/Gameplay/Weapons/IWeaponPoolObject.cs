using System;
using UnityEngine;

namespace Nutmeg.Runtime.Gameplay.Weapons
{
    public interface IWeaponPoolObject
    {
        public void SetReleaseAction(Action<GameObject> action);
    }
}