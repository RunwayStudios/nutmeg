using Nutmeg.Runtime.Gameplay.Combat;
using Nutmeg.Runtime.Gameplay.Zombies;
using UnityEngine;

[System.Serializable]
public class WaitForSurvivingZombiesWavePart : ZombieWavePart
{
    [SerializeField] private int numSurvivingZombies = 0;
    
    public override void Start()
    {
    }

    public override bool Update()
    {
        if (!CombatEntityManager.Main.activeGroups.TryGetValue(CombatGroup.Zombie, out var zombies))
            return true;

        return zombies.Count <= numSurvivingZombies;
    }
}