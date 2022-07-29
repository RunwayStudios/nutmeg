using System;
using UnityEngine;

namespace Nutmeg.Runtime.Gameplay.Player
{
    public class PlayerAnimationController : MonoBehaviour
    {
        [SerializeField] private Animator animator;

        private string currentAnimation;
        private void Update()
        {
        }

        public void UpdateFloatParam(string param, float amount)
        {
            animator.SetFloat(param, amount);
        }

        public void PlayAnimation(string animation)
        {
            if(currentAnimation == animation) return;
            
            animator.Play(animation);

            currentAnimation = animation;
        }

        public void PlayBlendAnimation(string blendAnimation)
        {
            if(currentAnimation == blendAnimation) return;
            animator.Play(blendAnimation);
            currentAnimation = blendAnimation;
        }
        
        public void SetNewAnimationParamBool(string param, bool b)
        {
            animator.SetBool(param, b);
        } 
    }
}