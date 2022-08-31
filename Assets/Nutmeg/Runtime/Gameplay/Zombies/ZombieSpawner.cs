using System;
using System.Collections.Generic;
using Nutmeg.Runtime.Gameplay.Combat.Zombies;
using UnityEngine;

namespace Nutmeg.Runtime.Gameplay.Zombies
{
    public class ZombieSpawner : MonoBehaviour
    {
        [SerializeField] private bool updateSpawningLocationsFromChildren = false;
        [SerializeField] private string spawningLocationsGoName = "Spawner";
        [SerializeField] private List<Transform> spawningLocations = new List<Transform>();
        
        [Space]
        [SerializeField] private bool testSingle = false;
        [SerializeField] private GameObject testZombiePrefab;
        
        
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            if (testSingle)
            {
                testSingle = false;

                if (spawningLocations.Count < 1)
                {
                    Debug.LogError("Spawners haven't been set up for zombieManager");
                    return;
                }
            
                int rndmSpawnerIndex = Mathf.FloorToInt((float)new System.Random().NextDouble() * spawningLocations.Count);
                Instantiate(testZombiePrefab, spawningLocations[rndmSpawnerIndex].position, new Quaternion());
            }
        }

        private void OnValidate()
        {
            if (updateSpawningLocationsFromChildren)
            {
                updateSpawningLocationsFromChildren = false;
                
                spawningLocations.Clear();
                for (int i = 0; i < transform.childCount; i++)
                {
                    Transform child = transform.GetChild(i);
                    spawningLocations.Add(child);
                    child.gameObject.name = spawningLocationsGoName + " " + i;
                }
            }
        }
    }
}
