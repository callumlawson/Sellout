using System;
using Assets.Framework.States;
using Assets.Framework.Systems;
using Assets.Scripts.States;
using System.Collections.Generic;
using Assets.Scripts.Util;
using UnityEngine;
using Assets.Framework.Entities;

namespace Assets.Scripts.Systems.Drinks
{
    class NewDrinkMakingSystem : IEntityManager, IInitSystem, IReactiveEntitySystem
    {
        private EntityStateSystem entitySystem;
        private DrinkUI ui;
        private Entity drinkTemplate;
        private DrinkState drinkState;

        private Entity Counter;

        public List<Type> RequiredStates()
        {
            return new List<Type> { typeof(CounterState) };
        }

        public void SetEntitySystem(EntityStateSystem ess)
        {
            entitySystem = ess;
        }

        public void OnEntityAdded(Entity entity)
        {
            Counter = entity;
        }

        public void OnEntityRemoved(Entity entity)
        {
            throw new NotSupportedException();
        }

        public void OnInit()
        {
            if (drinkTemplate == null)
            {
                drinkTemplate = entitySystem.CreateEntity(new List<IState>
                {
                    new DrinkState()
                });
            }
            drinkState = drinkTemplate.GetState<DrinkState>();
           
            EventSystem.StartDrinkMakingEvent += OnStartMakingDrink;
            EventSystem.EndDrinkMakingEvent += OnStopMakingDrink;
        }

        private void OnStartMakingDrink()
        {
          //TODO
        }

        private void OnStopMakingDrink()
        {
            drinkState.Clear();
        }

        private void OnDecrementIngredient(Ingredient ingredient)
        {
            drinkState.ChangeIngredientAmount(ingredient, -1);
        }

        private void OnIncrementIngredient(Ingredient ingredient)
        {
            drinkState.ChangeIngredientAmount(ingredient, 1);
        }
        
        private void OnMixEvent()
        {
            var player = StaticStates.Get<PlayerState>().Player;
            if (player.GetState<HierarchyState>().Child == null)
            {
                var drink = MakeDrink(drinkState);
                EventSystem.ParentingRequestEvent.Invoke(new ParentingRequest { EntityFrom = null, EntityTo = player, Mover = drink });
                drinkState.Clear();
            }
            else
            {
                Debug.Log("Unable to make a drink because the player is already holding one!");
            }
        }

        private Entity MakeDrink(DrinkState template)
        {
            return entitySystem.CreateEntity(new List<IState>
            {
                new PrefabState(Prefabs.Drink),
                new DrinkState(template),
                new PositionState(new Vector3(-9.68f, 1.27f, -14.27f)),
                new HierarchyState()
            });
        }
    }
}
