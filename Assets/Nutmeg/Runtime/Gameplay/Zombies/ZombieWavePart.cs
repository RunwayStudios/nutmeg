using UnityEngine;

namespace Nutmeg.Runtime.Gameplay.Zombies
{
    public abstract class ZombieWavePart : ScriptableObject
    {
        public abstract void StartPart();

        public abstract void Update();

        public abstract bool Finished();
    }
}
