using System.Collections.Generic;
using Nutmeg.Runtime.Gameplay.Combat;
using Nutmeg.Runtime.Gameplay.Combat.CombatModules;
using Nutmeg.Runtime.Gameplay.Money;
using Nutmeg.Runtime.Utility.Effects;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

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
        [SerializeField] private List<GameObject> skins = new List<GameObject>();

        private NavMeshAgent navMeshAgent;
        private Animator animator;

        // Start is called before the first frame update
        void Start()
        {
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
            }

            SetAnimationState("walk");
        }

        // Update is called once per frame
        void Update()
        {
            UpdateDecay();
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
                transform.position = new Vector3(transform.position.x, transform.position.y + decayDisplacement * (Time.deltaTime / decayDuration), transform.position.z);
            }
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
                navMeshAgent.isStopped = true;
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
                navMeshAgent.isStopped = false;
        }

        public void OnDeath()
        {
            SetAnimationState("die");
            SetAnimationState("attack", false);
            SetAnimationState("walk", false);

            Collider[] colliders = GetComponentsInChildren<Collider>();
            foreach (Collider c in colliders)
            {
                c.enabled = false;
            }

            GetComponent<NavMeshAgent>().enabled = false;

            if (NetworkManager.Singleton.IsServer)
            {
                MoneyManager.Main.AddBalance(killReward);
                Decay();
            }
        }
    }
}