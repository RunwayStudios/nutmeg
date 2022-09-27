using UnityEngine;

namespace Nutmeg.Runtime.Gameplay.Weapons
{
    public abstract class WeaponComponent : MonoBehaviour
    {
        public abstract T Get<T>();

    }
}