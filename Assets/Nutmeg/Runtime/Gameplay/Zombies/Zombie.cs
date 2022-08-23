using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Nutmeg.Runtime.Gameplay.Zombies
{
    public class Zombie : MonoBehaviour
    {
        [SerializeField]
        private List<GameObject> skins = new List<GameObject>();

        [SerializeField] private NavMeshAgent navMeshAgent;

        // Start is called before the first frame update
        void Start()
        {
            if (skins.Count < 1)
            {
                Debug.LogError("Skins haven't been set up for zombie");
                return;
            }

            int rndmSkinIndex = Mathf.FloorToInt((float)new System.Random().NextDouble() * skins.Count);
            skins[rndmSkinIndex].SetActive(true);


            if (navMeshAgent == null)
                navMeshAgent = GetComponent<NavMeshAgent>();

            navMeshAgent.SetDestination(new Vector3(0, 0, 0));
            
            
            ZombieManager.Main.activeZombies.Add(transform);
        }

        // Update is called once per frame
        void Update()
        {
        }
    }
}