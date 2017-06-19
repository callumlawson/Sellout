using System;
using System.Collections.Generic;
using Assets.Framework.Entities;
using Assets.Framework.States;
using Assets.Framework.Systems;
using Assets.Scripts.States;
using Assets.Scripts.Util;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.Scripts.Systems
{
    public class CharacterControllerSystem : IPhysicsFrameEntitySystem, IFrameEntitySystem, IReactiveEntitySystem
    {
        private PlayerState playerState;
        private PathfindingState playerPathfindingState;

        public List<Type> RequiredStates()
        {
            return new List<Type>{typeof(IsPlayerState), typeof(PathfindingState) };
        }

        public void OnFrame(List<Entity> matchingEntities)
        {
            if (playerState == null)
            {
                playerState = StaticStates.Get<PlayerState>();
                return;
            }
            playerPathfindingState.IsActive = playerState.CutsceneControlLock || StaticStates.Get<DayPhaseState>().CurrentDayPhase == DayPhase.Open;
        }

        public void OnPhysicsFrame(List<Entity> matchingEntities)
        {
            foreach (var entity in matchingEntities) //Just the player.
            {
                var ourRigidbody = entity.GameObject.GetComponent<Rigidbody>();

                if (playerPathfindingState.IsActive)
                {
                    if (ourRigidbody != null)
                    {
                        Object.Destroy(ourRigidbody);
                    }
                    return;
                }

                if (ourRigidbody == null)
                {
                    ourRigidbody = entity.GameObject.AddComponent<Rigidbody>();
                    ourRigidbody.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
                }

                if (playerState.PlayerStatus != PlayerStatus.FreeMove)
                {
                    return;
                }

                var x = -Input.GetAxisRaw("Vertical");
                var z = Input.GetAxisRaw("Horizontal");
                var rawInputVec = new Vector3(x, 0, z);
                var inputVec = rawInputVec.normalized * Constants.PlayerWalkSpeed;
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    inputVec *= Constants.PlayerRunMultiplier;
                }
                var speed = ourRigidbody.velocity.magnitude;
                ourRigidbody.AddForce(inputVec - ourRigidbody.velocity, ForceMode.VelocityChange);
                if (speed > 0.2f && rawInputVec.magnitude > 0.1f)
                {
                    entity.GameObject.transform.rotation = Quaternion.Slerp(entity.GameObject.transform.rotation, Quaternion.LookRotation(inputVec), Time.deltaTime * 5);
                }
            }
        }

        public void OnEntityAdded(Entity entity)
        {
            playerPathfindingState = entity.GetState<PathfindingState>();
        }

        public void OnEntityRemoved(Entity entity)
        {
            //Do nothing
        }
    }
}
