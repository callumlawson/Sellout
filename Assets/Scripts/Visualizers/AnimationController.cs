using System;
using System.Linq;
using Assets.Framework.Entities;
using Assets.Scripts.States;
using UnityEngine;
using UnityEngine.AI;
using AnimationEvent = Assets.Scripts.Util.AnimationEvent;

namespace Assets.Scripts.Visualizers
{
    class AnimationController : MonoBehaviour, IEntityVisualizer
    {
        private NavMeshAgent agent;
        private Animator animator;

        private PersonAnimationState personAnimationState;

        public void OnStartRendering(Entity entity)
        {
            agent = GetComponent<NavMeshAgent>();
            animator = GetComponentInChildren<Animator>();
            personAnimationState = entity.GetState<PersonAnimationState>();
            personAnimationState.TriggerAnimation += OnTriggerAnimation;
            personAnimationState.ResetAnimationState += OnResetAnimationState;
        }

        private void OnResetAnimationState()
        {
            //Hard set the state machine to the default "Idle" state.
            Enum.GetValues(typeof(AnimationEvent)).Cast<AnimationEvent>().ToList().ForEach(animationEvent => animator.ResetTrigger(animationEvent.ToString()));
            animator.Play("Idle");
        }

        private void OnTriggerAnimation(AnimationEvent animationEvent)
        {
            animator.SetTrigger(animationEvent.ToString());
        }

        public void OnFrame()
        {
            var currentSpeed = agent.velocity.sqrMagnitude;
            animator.SetFloat("Velocity", currentSpeed);
            animator.SetBool("Moving", currentSpeed > 0.1f);
        }

        public void OnStopRendering(Entity entity)
        {
            //Do nothing.
        }
    }
}
