using System;
using System.Collections.Generic;
using Assets.Framework.Entities;
using Assets.Framework.States;
using Assets.Framework.Systems;
using Assets.Scripts.States;
using Assets.Scripts.Util;

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

            if (playerState.Player.GetState<InventoryState>().Child != null && entityName == Prefabs.Beer)
            {
                interactiveState.CurrentlyInteractive = false;
                return;
            }

            if (playerState.Player.GetState<InventoryState>().Child != null &&
                playerState.Player.GetState<InventoryState>().Child.GetState<PrefabState>().PrefabName == Prefabs.DispensingBottle)
            {
                interactiveState.CurrentlyInteractive = entityName == Prefabs.Drink || entityName == Prefabs.DrinkSurface;
                return;
            }

            if (playerState.Player.GetState<InventoryState>().Child != null &&
                playerState.Player.GetState<InventoryState>().Child.GetState<PrefabState>().PrefabName == Prefabs.Drink)
            {
                interactiveState.CurrentlyInteractive = entityName == Prefabs.IngredientDispenser || 
                                                        entityName == Prefabs.DrinkSurface ||
                                                        entityName == Prefabs.Person ||
                                                        entityName == Prefabs.Washup;
                return;
            }

            if (playerState.Player.GetState<InventoryState>().Child != null &&
                playerState.Player.GetState<InventoryState>().Child.GetState<PrefabState>().PrefabName == Prefabs.Drugs)
            {
                interactiveState.CurrentlyInteractive = entityName == Prefabs.Washup ||
                                                        entityName == Prefabs.Cubby ||
                                                        entityName == Prefabs.Person;
                return;
            }

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
                interactiveState.CurrentlyInteractive = true;
            }
            if (entityName == Prefabs.Counter)
            {
                interactiveState.CurrentlyInteractive = playerState.PlayerStatus != PlayerStatus.Bar;
            }
            if (entityName == Prefabs.IngredientDispenser)
            {
                interactiveState.CurrentlyInteractive = 
                    playerState.Player.GetState<InventoryState>().Child != null &&
                    playerState.Player.GetState<InventoryState>().Child.GetState<PrefabState>().PrefabName == Prefabs.Drink;
            }
            if (entityName == Prefabs.Washup)
            {
                interactiveState.CurrentlyInteractive = playerState.Player.GetState<InventoryState>().Child != null;
            }
            if (entityName == Prefabs.Cubby)
            {
                interactiveState.CurrentlyInteractive =
                    (playerState.Player.GetState<InventoryState>().Child == null && 
                    entity.GetState<InventoryState>().Child != null) ||
                    (playerState.Player.GetState<InventoryState>().Child != null && 
                    Prefabs.CubbyPlaceablePrefabs.Contains(playerState.Player.GetState<InventoryState>().Child.GetState<PrefabState>().PrefabName));
            }
            if (entityName == Prefabs.ReceiveSpot)
            {
                interactiveState.CurrentlyInteractive = 
                    entity.GetState<InventoryState>().Child != null && 
                    playerState.Player.GetState<InventoryState>().Child == null;
            }
            if (entityName == Prefabs.DrinkSurface)
            {
                interactiveState.CurrentlyInteractive =
                    (playerState.Player.GetState<InventoryState>().Child != null &&
                    Prefabs.SurfacePlaceablePrefabs.Contains(playerState.Player.GetState<InventoryState>().Child.GetState<PrefabState>().PrefabName)) ||
                    playerState.PlayerStatus != PlayerStatus.Bar;
            }
        }
    }
}