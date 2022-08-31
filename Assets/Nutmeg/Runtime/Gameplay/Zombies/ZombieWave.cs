using UnityEngine;

namespace Nutmeg.Runtime.Gameplay.Zombies
{
    [CreateAssetMenu(menuName = "Zombies/ZombieWave")]
    public class ZombieWave : ScriptableObject
    {
        public ZombieWavePart[] parts;
    }
}
