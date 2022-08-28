using System.Collections.Generic;
using UnityEngine;

namespace Nutmeg.Runtime.Gameplay.Zombies
{
    public class ZombieManager : MonoBehaviour
    {
        public static ZombieManager Main;
        
        // [HideInInspector]
        public List<ZombiePositionStruct> activeZombies = new List<ZombiePositionStruct>();

        public List<Transform> spawners = new List<Transform>();


        [SerializeField] private GameObject testZombiePrefab;
        [SerializeField] private bool test = false;
        
        private void Awake()
        {
            Main = this;
        }
        
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            if (test)
            {
                test = false;

                if (spawners.Count < 1)
                {
                    Debug.LogError("Spawners haven't been set up for zombieManager");
                    return;
                }
            
                int rndmSpawnerIndex = Mathf.FloorToInt((float)new System.Random().NextDouble() * spawners.Count);
                Instantiate(testZombiePrefab, spawners[rndmSpawnerIndex].position, new Quaternion());
            }
        }


        public struct ZombiePositionStruct
        {
            public Vector3 pos;
            public Zombie zombie;

            public ZombiePositionStruct(Vector3 pos, Zombie zombie)
            {
                this.pos = pos;
                this.zombie = zombie;
            }
        }
    }
}
