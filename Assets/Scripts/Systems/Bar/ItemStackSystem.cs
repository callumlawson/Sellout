using System;
using UnityEngine;
using Assets.Framework.Systems;
using Assets.Scripts.States;
using System.Collections.Generic;
using Assets.Framework.Entities;
using Assets.Scripts.States.Bar;
using Assets.Scripts.Visualizers.Bar;

namespace Assets.Scripts.Systems.Bar
{
    class ItemStackSystem : IInitSystem, IEntityManager, IReactiveEntitySystem, IEndInitSystem
    {
        private EntityStateSystem entitySystem;

        private List<Entity> stacks = new List<Entity>();

        private bool finishedInitialization = false;

        public void OnInit()
        {
            EventSystem.TakeStackItem += OnTakeStackItem;
        }

        public void SetEntitySystem(EntityStateSystem ess)
        {
            entitySystem = ess;
        }
        public void OnEntityAdded(Entity stack)
        {
            if (!finishedInitialization)
            {
                stacks.Add(stack);
            }
            else
            {
                CreateNewItemInStack(stack);
            }
        }

        public void OnEndInit()
        {
            foreach (var stack in stacks)
            {
                CreateNewItemInStack(stack);
            }
            finishedInitialization = true;
        }

        public void OnEntityRemoved(Entity entity)
        {
           //Do Nothing
        }

        public List<Type> RequiredStates()
        {
            return new List<Type> { typeof(ItemStackState) };
        }

        private void OnTakeStackItem(TakeStackItemRequest request)
        {
            var requester = request.Requester;
            var stack = request.Stack;

            if (!stack.HasState<InventoryState>())
            {
                Debug.LogError("A requester requested to take a stack item but the stack doesn't have an inventory state!");
                return;
            }

            if (!requester.HasState<InventoryState>())
            {
                Debug.LogError("A requester requested to take a stack item but the requester doesn't have an inventory state!");
                return;
            }

            var stackItem = stack.GetState<InventoryState>().Child;

            if (stackItem == null)
            {
                Debug.LogError("A requester requested to take a stack item but there is no item!");
                return;
            }

            EventSystem.ParentingRequestEvent.Invoke(new ParentingRequest { EntityFrom = stack, EntityTo = requester, Mover = stackItem });

            if (stack.GetState<InventoryState>().Child == null)
            {
                CreateNewItemInStack(stack);
            }
        }
        
        private void CreateNewItemInStack(Entity stack)
        {
            var states = stack.GameObject.GetComponent<ItemStackVisualizer>().GetNewStackItem();

            var newItem = entitySystem.CreateEntity(states);

            EventSystem.ParentingRequestEvent.Invoke(new ParentingRequest { EntityFrom = null, EntityTo = stack, Mover = newItem });

            if (stack.GetState<InventoryState>().Child != newItem)
            {
                Debug.LogError("Tried to add the newly created item to the stack but it failed!");
                EntityStateSystem.Instance.RemoveEntity(newItem);
            }
        }

    }
}
