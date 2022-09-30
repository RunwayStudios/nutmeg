using System.Collections.Generic;
using IngameDebugConsole;
using Unity.Netcode;
using UnityEngine;

namespace Nutmeg.Runtime.Gameplay.Zombies
{
    public class ZombieSpawner : NetworkBehaviour
    {
        public static ZombieSpawner Main;
        
        
        [SerializeField] private bool updateSpawningLocationsFromChildren = false;
        [SerializeField] private string spawningLocationsGoName = "Spawner";
        [SerializeField] private List<Transform> spawningLocations = new List<Transform>();

        [Space] 
        [SerializeField] private bool testWave;
        private bool spawningWave;
        [SerializeField] private ZombieWave[] zombieWaves;
        [SerializeField] private int waveIndex = 0;
        private ZombieWave curWave;
        [SerializeField] private int wavePartIndex = 0;
        private ZombieWavePart curWavePart;
        

        private bool currentlySpawning;


        private void Awake()
        {
            Main = this;
        }

        // Start is called before the first frame update
        void Start()
        {
            DebugLogConsole.AddCommand<int>("Zombies.StartWave", "Start Wave", StartWaveServerRpc, "waveIndex");
        }

        // Update is called once per frame
        void Update()
        {
            if (testWave)
            {
                testWave = false;
                StartWave(waveIndex);
            }
            
            UpdateWave();
        }


        [ServerRpc(RequireOwnership = false)]
        private void StartWaveServerRpc(int index)
        {
            StartWave(index);
        }

        private void StartWave(int index)
        {
            if (index >= zombieWaves.Length)
            {
                currentlySpawning = false;
                return;
            }
            
            waveIndex = index;
            curWave = zombieWaves[waveIndex];
            wavePartIndex = 0;
            curWavePart = curWave.parts[wavePartIndex];
            curWavePart.Start();

            currentlySpawning = true;
        }

        private void NextWavePart()
        {
            wavePartIndex++;
            if (wavePartIndex >= curWave.parts.Length)
            {
                currentlySpawning = false;
                // automatically start with next wave
                // StartWave(waveIndex + 1);
                return;
            }

            curWavePart = curWave.parts[wavePartIndex];
            curWavePart.Start();
        }

        private void UpdateWave()
        {
            // if but immediately update the next wave part and check if it's done
            while (currentlySpawning && curWavePart.Update())
            {
                NextWavePart();
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


        public List<Transform> SpawningLocations => spawningLocations;
    }
}
