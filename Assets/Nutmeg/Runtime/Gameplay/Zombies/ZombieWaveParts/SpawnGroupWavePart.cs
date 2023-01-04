using System.Collections.Generic;
using Nutmeg.Runtime.Gameplay.Zombies;
using Unity.Netcode;
using UnityEngine;

public class SpawnGroupWavePart : ZombieWavePart
{
    [SerializeField] private GameObject zombiePrefab;
    [SerializeField] private int groupSize = 1;

    public override void Start()
    {
    }

    public override bool Update()
    {
        List<Transform> spawningLocations = ZombieSpawner.Main.SpawningLocations;
        Vector3 position = spawningLocations[Random.Range(0, spawningLocations.Count)].position;
        for (int i = 0; i < groupSize; i++)
        {
            GameObject go = Object.Instantiate(zombiePrefab, position, new Quaternion());
            go.GetComponentInChildren<NetworkObject>().Spawn(true);
        }

        return true;
    }
}