using Nutmeg.Runtime.Gameplay.Zombies;
using UnityEngine;

public class WavePartGroup : ZombieWavePart
{
    [SerializeReference, SerializeReferenceButton]
    private ZombieWavePart[] parts;

    private int index;

    public override void Start()
    {
        index = 0;
        parts[0].Start();
    }

    public override bool Update()
    {
        while (index < parts.Length && parts[index].Update())
        {
            index++;
            if (index >= parts.Length)
                return true;

            parts[index].Start();
        }

        return false;
    }
}