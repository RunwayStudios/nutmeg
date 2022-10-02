using System.Collections.Generic;
using Nutmeg.Runtime.Gameplay.Combat;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

namespace Nutmeg.Runtime.Gameplay.Zombies
{
    public class Zombie : MonoBehaviour
    {
        [SerializeField] private List<GameObject> skins = new List<GameObject>();

        [SerializeField] private float decayDelay = 5f;
        [SerializeField] private float decayDuration = 10f;
        [SerializeField] private float decayDisplacement = -1;
        private bool decaying;
        private float decayStart;
        private float decayingOriginalY;

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
                // todo set base center/hut?
                navMeshAgent.SetDestination(new Vector3(0, 0, 0));
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
                transform.position = new Vector3(transform.position.x,
                    transform.position.y + decayDisplacement * (Time.deltaTime / decayDuration), transform.position.z);
            }
        }

        private void Decay()
        {
            decaying = true;
            decayStart = Time.time;
            decayingOriginalY = transform.position.y;
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

        public void OnStopAttacking()
        {
            SetAnimationState("walk");

            if (NetworkManager.Singleton.IsServer)
                navMeshAgent.isStopped = false;
        }

        public void OnDeath()
        {
            SetAnimationState("walk", false);
            SetAnimationState("die");

            Collider[] colliders = GetComponentsInChildren<Collider>();
            foreach (Collider c in colliders)
            {
                c.enabled = false;
            }

            GetComponent<NavMeshAgent>().enabled = false;

            if (NetworkManager.Singleton.IsServer)
                Decay();
        }
    }
}