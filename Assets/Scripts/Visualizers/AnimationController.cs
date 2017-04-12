using Assets.Framework.Entities;
using Assets.Scripts.States;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.Visualizers
{
    class AnimationController : MonoBehaviour, IEntityVisualizer
    {
        private NavMeshAgent agent;
        private Animator animator;
        private States.PersonAnimationState personAnimationState;

        public void OnStartRendering(Entity entity)
        {
            agent = GetComponent<NavMeshAgent>();
            animator = GetComponentInChildren<Animator>();
            personAnimationState = entity.GetState<States.PersonAnimationState>();
        }

        public void OnFrame()
        {
            animator.SetBool("Sitting", personAnimationState.CurrentStatus == AnimationStatus.Sitting);
            if (personAnimationState.CurrentStatus == AnimationStatus.Moving)
            {
                var currentSpeed = agent.velocity.sqrMagnitude;
                animator.SetFloat("Velocity", currentSpeed);
                animator.SetBool("Moving", currentSpeed > 0.1f);
            }
            else
            {
                animator.SetFloat("Velocity", 0.0f);
                animator.SetBool("Moving", false);
            }
        }

        public void OnStopRendering(Entity entity)
        {
            //Do nothing.
        }
    }
}
