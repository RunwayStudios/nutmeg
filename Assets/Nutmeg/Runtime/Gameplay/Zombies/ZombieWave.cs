using System.Collections.Generic;
using UnityEngine;

namespace Nutmeg.Runtime.Gameplay.Zombies
{
    [CreateAssetMenu(menuName = "Zombies/ZombieWave")]
    public class ZombieWave : ScriptableObject
    {
        [SerializeReference, SerializeReferenceButton]
        public ZombieWavePart[] parts;
    }
}
