using System;
using UnityEngine;

namespace Nutmeg.Runtime.Gameplay.Player
{
    public class PlayerAnimationController : MonoBehaviour
    {
        [SerializeField] private Animator animator;

        private string currentAnimation;

        private float animationFadeTime;

        private void Start()
        {
        }
        
        private void Update()
        {
        }

        public void SetAnimationFadeTime(float time) => animationFadeTime = time;
        
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

        public void PlayCrossAnimation(string animation, int layer) =>
            PlayCrossAnimation(animation, animationFadeTime, layer);
        
        public void PlayCrossAnimation(string animation, float fadeTime, int layer)
        {
            if(currentAnimation == animation /*|| animator.IsInTransition(layer)*/) return;
            
            animator.CrossFade(animation, fadeTime, layer);
            
            currentAnimation = animation;
        }

        public void SetNewAnimationParamBool(string param, bool b)
        {
            animator.SetBool(param, b);
        } 
    }
}