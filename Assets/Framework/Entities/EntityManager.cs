using System;
using System.Collections.Generic;
using System.Reflection;
using Assets.Framework.States;
using Assets.Framework.Util;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.Framework.Entities
{
    public class EntityManager
    {
        private readonly Bag<Entity> entities = new Bag<Entity>();
        private readonly Dictionary<Type, Bag<IState>> statesByType = new Dictionary<Type, Bag<IState>>();
        private int nextAvailableId;

        //TODO: Premature optimisation is the devil's work... This to be replaced by init time refelection.
        private readonly MethodInfo addStateMethod = typeof(EntityManager).GetMethod("AddState");
        private readonly MethodInfo getStateMethod = typeof(EntityManager).GetMethod("GetState", new[] { typeof(Entity) });

        public Entity BuildEntity(IEnumerable<IState> states)
        {
            var entity = CreateEmptyEntity();
            foreach (var state in states)
            {
                var genericAdd = addStateMethod.MakeGenericMethod(state.GetType());
                genericAdd.Invoke(this, new object[] { entity, state });

                //Yuck, special case - might want to reconsider.
                if (state is PrefabState)
                {
                    var prefabState = state as PrefabState;
                    var go = SimplePool.Spawn(Resources.Load(prefabState.PrefabName) as GameObject);
                    var entityIdComponent = go.AddComponent<EntityIdComponent>();
                    entityIdComponent.EntityId = entity.EntityId;
                    entity.GameObject = go;
                }
            }
            return entity;
        }

        public Entity GetEntity(int entityId)
        {
            return entities[entityId];
        }

        public void DeleteEntity(Entity entity)
        {
            if (entity.GameObject != null)
            {
                SimplePool.Despawn(entity.GameObject);
            }
            RemoveStatesForEntity(entity);
            entities[entity.EntityId] = null;
        }

        [UsedImplicitly]
        public void AddState<T>([NotNull] Entity entity, [NotNull] IState state) where T : IState
        {
            Bag<IState> componentsOfType;
            var listInitialized = statesByType.TryGetValue(typeof(T), out componentsOfType);

            if (!listInitialized)
            {
                componentsOfType = new Bag<IState>();
                statesByType.Add(typeof(T), componentsOfType);
            }

            componentsOfType.Set(entity.EntityId, state);
        }

        //For debug only!
        public List<IState> GetStates(Entity entity)
        {
            var results = new List<IState>();
            foreach (var states in statesByType.Values)
            {
                if (states.Get(entity.EntityId) != null)
                {
                    results.Add(states.Get(entity.EntityId));
                }
            }
            return results;
        }

        public IState GetState([NotNull] Entity entity, Type stateType)
        {
            var genericAdd = getStateMethod.MakeGenericMethod(stateType);
            return (IState) genericAdd.Invoke(this, new object[] { entity });
        }

        public T GetState<T>([NotNull] Entity entity)
        {
            Bag<IState> statesOfType;
            var hasType = statesByType.TryGetValue(typeof(T), out statesOfType);
            if (hasType)
            {
                return (T)statesOfType.Get(entity.EntityId);
            }
            return default(T);
        }

        public IEnumerable<Entity> GetEntitiesWithState<T>() where T : IState
        {
            var results = new List<Entity>();
            foreach (var entity in entities)
            {
                if (entity.HasState<T>())
                {
                    results.Add(entity);
                }
            }
            return results;
        }

        private Entity CreateEmptyEntity()
        {
            var entityId = nextAvailableId;
            nextAvailableId++;
            var entity = new Entity(this, entityId);
            entities.Set(entityId, entity);
            return entity;
        }

        private void RemoveStatesForEntity([NotNull] Entity entity)
        {
            foreach (var stateBag in statesByType.Values)
            {
                if (stateBag.Get(entity.EntityId) != null)
                {
                    stateBag.Set(entity.EntityId, null);
                }
            }
        }
    }
}
