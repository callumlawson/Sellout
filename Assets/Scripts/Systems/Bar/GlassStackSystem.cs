using System;
using UnityEngine;
using Assets.Framework.Systems;
using Assets.Scripts.States;
using System.Collections.Generic;
using Assets.Framework.States;
using Assets.Scripts.Util;
using Assets.Framework.Entities;
using Assets.Scripts.States.Bar;

namespace Assets.Scripts.Systems.Bar
{
    class GlassStackSystem : IInitSystem, IEntityManager, IReactiveEntitySystem, IEndInitSystem
    {
        private EntityStateSystem entitySystem;

        private Entity glassStack;

        public void OnInit()
        {
            EventSystem.TakeGlass += OnTakeGlass;
        }

        public void SetEntitySystem(EntityStateSystem ess)
        {
            entitySystem = ess;
        }
        public void OnEntityAdded(Entity stack)
        {
            glassStack = stack;
        }

        public void OnEndInit()
        {
            CreateNewGlassInStack(glassStack);
        }

        public void OnEntityRemoved(Entity entity)
        {
           //Do Nothing
        }

        public List<Type> RequiredStates()
        {
            return new List<Type> { typeof(GlassStackState) };
        }

        private void OnTakeGlass(TakeGlassRequest request)
        {
            var requester = request.Requester;
            var stack = request.stack;

            if (!stack.HasState<InventoryState>())
            {
                Debug.LogError("A requester requested to take a glass but the stack doesn't have an inventory state!");
                return;
            }

            if (!requester.HasState<InventoryState>())
            {
                Debug.LogError("A requester requested to take a glass but the requester doesn't have an inventory state!");
                return;
            }

            var glass = stack.GetState<InventoryState>().Child;

            if (glass == null)
            {
                Debug.LogError("A requester requested to take a glass but there is no glass!");
                return;
            }

            EventSystem.ParentingRequestEvent.Invoke(new ParentingRequest { EntityFrom = stack, EntityTo = requester, Mover = glass });

            if (stack.GetState<InventoryState>().Child == null)
            {
                CreateNewGlassInStack(stack);
            }
        }
        
        private void CreateNewGlassInStack(Entity stack)
        {
            var glass = entitySystem.CreateEntity(new List<IState>
            {
                new PrefabState(Prefabs.Drink),
                new DrinkState(new DrinkState()),
                new PositionState(stack.GameObject.transform.position),
                new InventoryState()
            });

            EventSystem.ParentingRequestEvent.Invoke(new ParentingRequest { EntityFrom = null, EntityTo = stack, Mover = glass });

            if (stack.GetState<InventoryState>().Child != glass)
            {
                Debug.LogError("Tried to add the newly created glass to the stack but it failed!");
                EntityStateSystem.Instance.RemoveEntity(glass);
            }
        }

    }
}
