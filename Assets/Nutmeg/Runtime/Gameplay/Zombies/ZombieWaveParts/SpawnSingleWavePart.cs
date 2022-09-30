using System.Collections.Generic;
using Nutmeg.Runtime.Gameplay.Zombies;
using Unity.Netcode;
using UnityEngine;

public class SpawnSingleWavePart : ZombieWavePart
{
    [SerializeField] private GameObject zombiePrefab;

    public override void Start()
    {
    }

    public override bool Update()
    {
        List<Transform> spawningLocations = ZombieSpawner.Main.SpawningLocations;
        Vector3 position = spawningLocations[Random.Range(0, spawningLocations.Count)].position;
        GameObject go = Object.Instantiate(zombiePrefab, position, new Quaternion());
        go.GetComponent<NetworkObject>().Spawn(true);
        
        return true;
    }
}