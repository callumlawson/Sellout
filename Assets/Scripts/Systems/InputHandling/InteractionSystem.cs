using System;
using System.Collections.Generic;
using Assets.Framework.Entities;
using Assets.Framework.States;
using Assets.Framework.Systems;
using Assets.Scripts.States;
using Assets.Scripts.Util;
using UnityEngine;

namespace Assets.Scripts.Systems.InputHandling
{
    internal class InteractionSystem : IFrameEntitySystem, IInitSystem
    {
        private PlayerState playerState;

        public List<Type> RequiredStates()
        {
            return new List<Type> { typeof(InteractiveState) };
        }

        public void OnInit()
        {
            playerState = StaticStates.Get<PlayerState>();
        }

        public void OnFrame(List<Entity> matchingEntities)
        {
            foreach (var entity in matchingEntities)
            {
                SetInteractive(entity);
            }
        }

        private void SetInteractive(Entity entity)
        {
            var entityName = entity.GetState<PrefabState>().PrefabName;
            var interactiveState = entity.GetState<InteractiveState>();

            if (entity.HasState<IsPersonState>() && entity.HasState<ConversationState>())
            {
                interactiveState.CurrentlyInteractive = 
                    playerState.PlayerStatus == PlayerStatus.Bar 
                    || entity.GetState<ConversationState>().Conversation != null
                    || playerState.Player.GetState<InventoryState>().Child != null;
            }
            if (entity.HasState<IsPlayerState>())
            {
                interactiveState.CurrentlyInteractive = false;
            }
            if (Prefabs.BarObjectPrefabs.Contains(entityName))
            {
                interactiveState.CurrentlyInteractive = playerState.PlayerStatus == PlayerStatus.Bar;
            }
            if (entityName == Prefabs.Counter)
            {
                interactiveState.CurrentlyInteractive = playerState.PlayerStatus != PlayerStatus.Bar;
            }
            if (entityName == Prefabs.Washup)
            {
                interactiveState.CurrentlyInteractive = playerState.Player.GetState<InventoryState>().Child != null;
            }
        }
    }
}