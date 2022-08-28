using System.Collections;
using System.Collections.Generic;
using Nutmeg.Runtime.Gameplay.BaseBuilding;
using Nutmeg.Runtime.Gameplay.Zombies;
using UnityEngine;

public class AttackingPlaceable : Placeable
{

    [SerializeField] private float attackRadius = 5f;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    #region Attacking

    protected virtual bool TryGetClosestZombieInRadius(out Zombie zombie)
    {
        zombie = null;
        List<ZombieManager.ZombiePositionStruct> zombies = ZombieManager.Main.activeZombies;
        float lowestDistance = float.MaxValue;
        bool found = false;
        for (int i = 0; i < zombies.Count; i++)
        {
            float distance = new Vector2(transform.position.x - zombies[i].pos.x, transform.position.z - zombies[i].pos.z).magnitude;
            if (distance < attackRadius && distance < lowestDistance)
            {
                lowestDistance = distance;
                zombie = zombies[i].zombie;
                found = true;
            }
        }

        return found;
    }

    #endregion
}
