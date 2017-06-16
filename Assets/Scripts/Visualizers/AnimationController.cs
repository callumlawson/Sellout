using System;
using System.Linq;
using Assets.Framework.Entities;
using Assets.Framework.States;
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
        private Rigidbody ourRigidbody;
        private bool isPlayer;
        private Entity entity;

        public void OnStartRendering(Entity entity)
        {
            this.entity = entity;
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

        public void FixedUpdate()
        {
            if (entity.HasState<IsPlayerState>() && ourRigidbody == null)
            {
                ourRigidbody = entity.GameObject.GetComponent<Rigidbody>();
            }

            var playerState = StaticStates.Get<PlayerState>();
            var velocity = Vector3.zero;
            
            if (entity.HasState<IsPlayerState>() && playerState != null && !playerState.CutsceneControlLock && ourRigidbody != null)
            {
                velocity = ourRigidbody.velocity;
            }
            else if (agent != null)
            {
                velocity = agent.desiredVelocity;
            }
            var speed = velocity.magnitude;
            animator.SetFloat("Velocity", speed);
            animator.SetBool("Moving", speed > 0.2f);
        }

        public void OnStopRendering(Entity entity)
        {
            //Do nothing.
        }

        public void OnFrame()
        {
            //Do nothing.
        }
    }
}
