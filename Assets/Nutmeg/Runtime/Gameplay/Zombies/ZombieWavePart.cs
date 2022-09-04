namespace Nutmeg.Runtime.Gameplay.Zombies
{
    [System.Serializable]
    public abstract class ZombieWavePart
    {
        public abstract void Start();

        /// <summary>
        /// update wave part
        /// </summary>
        /// <returns>whether wave part finished</returns>
        public abstract bool Update();
    }
}
