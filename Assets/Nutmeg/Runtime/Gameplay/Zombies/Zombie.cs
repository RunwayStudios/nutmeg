using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Nutmeg.Runtime.Gameplay.Zombies
{
    public class Zombie : MonoBehaviour
    {
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
            Debug.Log(animator.isInitialized);

            navMeshAgent = GetComponent<NavMeshAgent>();

            // todo set base center/hut?
            navMeshAgent.SetDestination(new Vector3(0, 0, 0));
            SetAnimationState("walk");
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        public void SetAnimationState(string parameter, bool value = true)
        {
            animator.SetBool(parameter, value);
        }
        
        public void OnStartAttacking()
        {
            navMeshAgent.isStopped = true;
            SetAnimationState("walk", false);
        }

        public void OnAttack()
        {
            SetAnimationState("attack");
        }
        
        public void OnStopAttacking()
        {
            navMeshAgent.isStopped = false;
            SetAnimationState("walk");
        }
    }
}