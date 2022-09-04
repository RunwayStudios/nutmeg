using Nutmeg.Runtime.Gameplay.Zombies;
using UnityEngine;

[System.Serializable]
public class MultiplyWavePart : ZombieWavePart
{
    [SerializeField] private int numMultiplications = 1;
    private int updatesLeft;

    [SerializeReference, SerializeReferenceButton]
    private ZombieWavePart wavePart;

    public override void Start()
    {
        updatesLeft = numMultiplications;

        if (updatesLeft > 0)
            wavePart.Start();
    }

    public override bool Update()
    {
        // if but immediately update the next wave part and check if it's done
        while (updatesLeft > 0 && wavePart.Update())
        {
            updatesLeft--;
            if (updatesLeft > 0)
                wavePart.Start();
        }

        return updatesLeft <= 0;
    }
}