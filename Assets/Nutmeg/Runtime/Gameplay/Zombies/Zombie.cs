using System;
using System.Collections.Generic;
using System.Linq;
using Nutmeg.Runtime.Gameplay.BaseBuilding;
using UnityEngine;
using UnityEngine.AI;

namespace Nutmeg.Runtime.Gameplay.Zombies
{
    public class Zombie : MonoBehaviour
    {
        [SerializeField] private List<GameObject> skins = new List<GameObject>();

        [SerializeField] private NavMeshAgent navMeshAgent;

        [Space] [Header("Obstacles")] [SerializeField]
        private float obstacleCheckInterval = 0.5f;

        [SerializeField] private Transform obstacleDetectorCenter;
        [SerializeField] private Vector3 obstacleDetectorHalfExtends = Vector3.one;
        private float lastObstacleCheck = 0f;
        [SerializeField] private List<Placeable> currentObstacles = new List<Placeable>();

        [Space] [Header("Attacking")] [SerializeField]
        private float attackInterval = 0.5f;

        [SerializeField] private float attackDamage = 5;
        private float lastAttackTry = 0f;
        private bool attacking = false;


        // Start is called before the first frame update
        void Start()
        {
            if (skins.Count < 1)
            {
                Debug.LogError("Skins haven't been set up for " + gameObject.name);
                return;
            }

            int rndmSkinIndex = Mathf.FloorToInt((float)new System.Random().NextDouble() * skins.Count);
            skins[rndmSkinIndex].SetActive(true);


            if (navMeshAgent == null)
                navMeshAgent = GetComponent<NavMeshAgent>();

            // todo set base center/hut?
            navMeshAgent.SetDestination(new Vector3(0, 0, 0));


            ZombieManager.Main.activeZombies.Add(new ZombieManager.ZombiePositionStruct(transform.position, this));
        }

        // Update is called once per frame
        void Update()
        {
            if (ShouldCheckObstacles())
                CheckObstacles();
            if (ShouldAttack())
                Attack();
        }


        #region Obstacles

        protected virtual void CheckObstacles()
        {
            int currentObstaclesStillInDetector = 0;
            bool newObstacle = false;
            List<Collider> colliders = Physics.OverlapBox(obstacleDetectorCenter.position, obstacleDetectorHalfExtends, obstacleDetectorCenter.rotation).ToList();
            List<Placeable> collidingPlaceables = new List<Placeable>();
            for (int i = colliders.Count - 1; i >= 0; i--)
            {
                Placeable placeable = colliders[i].gameObject.GetComponent<Placeable>();
                if (!placeable)
                {
                    colliders.RemoveAt(i);
                    continue;
                }

                collidingPlaceables.Add(placeable);

                if (currentObstacles.Contains(placeable))
                {
                    currentObstaclesStillInDetector++;
                    continue;
                }

                newObstacle = true;
                currentObstacles.Add(placeable);
                NewObstacleAdded(placeable);
            }

            if (!newObstacle && currentObstaclesStillInDetector < currentObstacles.Count)
            {
                for (int i = currentObstacles.Count - 1; i >= 0; i--)
                {
                    if (!collidingPlaceables.Contains(currentObstacles[i]))
                    {
                        Placeable placeable = currentObstacles[i];
                        currentObstacles.Remove(placeable);
                        OldObstacleRemoved(placeable);
                    }
                }
            }
        }

        protected virtual bool ShouldCheckObstacles()
        {
            if (attacking)
                return false;
            
            if (lastObstacleCheck + obstacleCheckInterval > Time.time)
                return false;

            lastObstacleCheck = Time.time;
            return true;
        }

        protected virtual void NewObstacleAdded(Placeable placeable)
        {
            if (CurrentObstaclesCount() < 2)
            {
                StartAttacking();
            }
        }

        protected virtual void OldObstacleRemoved(Placeable placeable)
        {
            if (CurrentObstaclesCount() < 1)
            {
                StopAttacking();
            }
        }

        private int CurrentObstaclesCount()
        {
            return currentObstacles.Count;
        }

        #endregion

        #region Attacking

        protected virtual bool ShouldAttack()
        {
            if (!attacking)
                return false;
            
            if (lastAttackTry + attackInterval > Time.time)
                return false;

            lastAttackTry = Time.time;
            return true;
        }

        protected virtual void StartAttacking()
        {
            navMeshAgent.isStopped = true;
            attacking = true;
        }

        protected virtual void StopAttacking()
        {
            navMeshAgent.isStopped = false;
            attacking = false;
        }

        protected virtual void Attack()
        {
            CheckObstacles();

            if (CurrentObstaclesCount() > 0)
                currentObstacles[0].Damage(attackDamage, Placeable.DamageType.Default);
        }

        #endregion

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.matrix = Matrix4x4.TRS(obstacleDetectorCenter.position, obstacleDetectorCenter.rotation, transform.lossyScale);
            Gizmos.DrawWireCube(Vector3.zero, obstacleDetectorHalfExtends * 2);
        }
    }
}