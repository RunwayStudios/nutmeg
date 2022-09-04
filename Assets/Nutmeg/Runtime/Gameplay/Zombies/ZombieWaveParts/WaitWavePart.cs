using Nutmeg.Runtime.Gameplay.Zombies;
using UnityEngine;

[System.Serializable]
public class WaitWavePart : ZombieWavePart
{
    [SerializeField] private float seconds = 1f;

    private float startedWait;

    public override void Start()
    {
        startedWait = Time.time;
    }

    public override bool Update()
    {
        return startedWait + seconds < Time.time;
    }
}