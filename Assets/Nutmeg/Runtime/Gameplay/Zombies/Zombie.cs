using System.Collections.Generic;
using Nutmeg.Runtime.Gameplay.Combat;
using Nutmeg.Runtime.Gameplay.Combat.CombatModules;
using Nutmeg.Runtime.Gameplay.Money;
using Nutmeg.Runtime.Utility.Effects;
using Nutmeg.Runtime.Utility.Networking;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Nutmeg.Runtime.Gameplay.Zombies
{
    public class Zombie : MonoBehaviour
    {
        [SerializeField] private int killReward = 0;
        [Space] [SerializeField] private float decayDelay = 5f;
        [SerializeField] private float decayDuration = 10f;
        [SerializeField] private float decayDisplacement = -1;
        private bool decaying;
        private float decayStart;

        [Space] [SerializeField] public Transform bulletSource;
        [SerializeField] private EffectSpawner[] damageEffects;
        [SerializeField] private EffectSpawner[] deathEffects;
        [SerializeField] private List<GameObject> skins = new List<GameObject>();

        [Space] [Header("Position Syncing")] [SerializeField]
        private ZombieNetworkTransform networkTransform;

        [SerializeField] private float posThreshold = 1f;
        [SerializeField] private float timeThreshold = 3f;
        [SerializeField] private float timedPositionThreshold = 0f;
        [SerializeField] private float firstPosUpdateDelay = .5f;
        private bool sentFirstPosUpdate;
        private float lastNetPosUpdateTime;
        private Vector3 lastNetPosUpdatePos;
        private bool updateNetworkPos = true;

        private float startTimestamp;

        private NavMeshAgent navMeshAgent;
        private Animator animator;


        // Start is called before the first frame update
        void Start()
        {
            startTimestamp = Time.time;
            
            if (skins.Count < 1)
            {
                Debug.LogError("Skins haven't been set up for " + gameObject.name);
                return;
            }

            int rndmSkinIndex = Random.Range(0, skins.Count);
            skins[rndmSkinIndex].SetActive(true);


            animator = GetComponent<Animator>();

            if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer)
            {
                navMeshAgent = GetComponent<NavMeshAgent>();
                // todo set base center/hut? also in PathfindFirstAttackerModule
                navMeshAgent.SetDestination(Vector3.zero);

                UpdateNetworkPosition();
            }

            SetAnimationState("walk");
        }

        // Update is called once per frame
        void Update()
        {
            CheckFirstPosUpdate();
            UpdateDecay();
            CheckForNetworkPositionUpdate();
        }


        private void CheckFirstPosUpdate()
        {
            if (!sentFirstPosUpdate && Time.time > startTimestamp + firstPosUpdateDelay)
            {
                sentFirstPosUpdate = true;
                UpdateNetworkPosition();
            }
        }

        private void UpdateDecay()
        {
            if (!decaying)
                return;

            if (Time.time > decayStart + decayDelay + decayDuration)
            {
                decaying = false;
                Destroy(gameObject);
                return;
            }

            if (Time.time > decayStart + decayDelay)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y + decayDisplacement * (Time.deltaTime / decayDuration),
                    transform.position.z);
            }
        }

        private void CheckForNetworkPositionUpdate()
        {
            if (!updateNetworkPos || !NetworkManager.Singleton.IsServer)
                return;

            Vector3 curPos = transform.position;

            if (Mathf.Abs(curPos.x - lastNetPosUpdatePos.x) > posThreshold ||
                Mathf.Abs(curPos.y - lastNetPosUpdatePos.y) > posThreshold ||
                Mathf.Abs(curPos.z - lastNetPosUpdatePos.z) > posThreshold ||
                timeThreshold < Time.time - lastNetPosUpdateTime &&
                (Mathf.Abs(curPos.x - lastNetPosUpdatePos.x) >= timedPositionThreshold ||
                 Mathf.Abs(curPos.y - lastNetPosUpdatePos.y) >= timedPositionThreshold ||
                 Mathf.Abs(curPos.z - lastNetPosUpdatePos.z) >= timedPositionThreshold))
            {
                UpdateNetworkPosition();
            }
        }

        private void UpdateNetworkPosition(bool stopped = false)
        {
            updateNetworkPos = !stopped;
            lastNetPosUpdatePos = transform.position;
            lastNetPosUpdateTime = Time.time;
            networkTransform.UpdateServerState(stopped);
        }

        private void Decay()
        {
            decaying = true;
            decayStart = Time.time;
        }

        private void SetAnimationState(string parameter, bool value = true)
        {
            animator.SetBool(parameter, value);
        }

        public void OnStartAttacking()
        {
            SetAnimationState("walk", false);

            if (NetworkManager.Singleton.IsServer)
            {
                navMeshAgent.isStopped = true;
                UpdateNetworkPosition(true);
            }
        }

        public void OnAttack()
        {
            SetAnimationState("attack");
        }

        public void RotateToTarget()
        {
            if (!GetComponent<CombatEntity>().TryGetModule(typeof(RadiusDetectorModule), out CombatModule module))
                return;

            CombatEntity target = ((DetectorModule)module).MostRecentTarget;
            if (!target)
                return;

            transform.LookAt(new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z));
        }

        public void OnStopAttacking()
        {
            SetAnimationState("walk");

            if (NetworkManager.Singleton.IsServer)
            {
                navMeshAgent.isStopped = false;
                UpdateNetworkPosition();
            }
        }

        public void OnDamage(DamageInfo info)
        {
            for (int i = 0; i < damageEffects.Length; i++)
            {
                damageEffects[i].TrySpawnEffect(info);
            }
        }

        public void OnDeath(DamageInfo info)
        {
            OnDeath();

            for (int i = 0; i < deathEffects.Length; i++)
            {
                deathEffects[i].TrySpawnEffect(info);
            }
        }

        public void OnDeath()
        {
            UpdateNetworkPosition(true);

            SetAnimationState("die");
            SetAnimationState("attack", false);
            SetAnimationState("walk", false);

            Collider[] colliders = GetComponentsInChildren<Collider>();
            foreach (Collider c in colliders)
            {
                c.enabled = false;
            }

            if (NetworkManager.Singleton.IsServer)
            {
                navMeshAgent.enabled = false;
                UpdateNetworkPosition();

                MoneyManager.Main.AddBalance(killReward);
                Decay();
            }
        }
    }
}