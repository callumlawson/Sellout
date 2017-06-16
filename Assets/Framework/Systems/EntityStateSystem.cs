using System.Collections.Generic;
using System.Runtime.Serialization;
using Assets.Framework.Entities;
using Assets.Framework.States;
using Assets.Framework.Util;

namespace Assets.Framework.Systems
{
    public class EntityStateSystem
    {
        //TODO: Remove this and replace with dependency injection
        public static EntityStateSystem Instance;

        private readonly Dictionary<IFilteredSystem, List<Entity>> activeEntitiesPerSystem = new Dictionary<IFilteredSystem, List<Entity>>();
        private readonly List<ITickEntitySystem> tickEntitySystems = new List<ITickEntitySystem>();
        private readonly List<ITickSystem> tickSystems = new List<ITickSystem>();
        private readonly List<IFrameEntitySystem> frameEntitySystems = new List<IFrameEntitySystem>();
        private readonly List<IPhysicsFrameEntitySystem> physicsFrameEntitySystems = new List<IPhysicsFrameEntitySystem>();
        private readonly List<IFrameSystem> updateSytems = new List<IFrameSystem>();
        private readonly List<IInitSystem> initSystems = new List<IInitSystem>();
        private readonly List<IEndInitEntitySystem> endInitEntitySystems = new List<IEndInitEntitySystem>();
        private readonly List<IEndInitSystem> endInitSystems = new List<IEndInitSystem>();
        private readonly List<IPausableSystem> pausableSystems = new List<IPausableSystem>();
        private readonly List<Entity> entitiesToRemove = new List<Entity>();
        private readonly EntityManager entityManager;

        private bool paused;

        public EntityStateSystem()
        {
            entityManager = new EntityManager();
            Instance = this;
        }

        public void AddSystem(ISystem system)
        {
            var tickEntitySystem = system as ITickEntitySystem;
            var frameEntitySystem = system as IFrameEntitySystem;
            var physicsFrameEntitySystem = system as IPhysicsFrameEntitySystem;
            var updateSystem = system as IFrameSystem;
            var tickSystem = system as ITickSystem;
            var fiteredSystem = system as IFilteredSystem;
            var entityManagerSystem = system as IEntityManager;
            var initSystem = system as IInitSystem;
            var endInitEntitySystem = system as IEndInitEntitySystem;
            var endInitSystem = system as IEndInitSystem;
            var pausableSystem = system as IPausableSystem;

            if (entityManagerSystem != null)
            {
                entityManagerSystem.SetEntitySystem(this);
            }

            if (tickEntitySystem != null)
            {
              tickEntitySystems.Add(tickEntitySystem);  
            }

            if (frameEntitySystem != null)
            {
                frameEntitySystems.Add(frameEntitySystem);
            }

            if (physicsFrameEntitySystem != null)
            {
                physicsFrameEntitySystems.Add(physicsFrameEntitySystem);
            }

            if (updateSystem != null)
            {
                updateSytems.Add(updateSystem);
            }

            if (tickSystem != null)
            {
                tickSystems.Add(tickSystem);
            }

            if (fiteredSystem != null)
            {
                activeEntitiesPerSystem.Add(fiteredSystem, new List<Entity>());
            }

            if (initSystem != null)
            {
                initSystems.Add(initSystem);
            }

            if (endInitEntitySystem != null)
            {
                endInitEntitySystems.Add(endInitEntitySystem);
            }

            if (endInitSystem != null)
            {
                endInitSystems.Add(endInitSystem);
            }

            if (pausableSystem != null)
            {
                pausableSystems.Add(pausableSystem);
            }
        }

        public void Init()
        {
            foreach (var system in initSystems)
            {
                system.OnInit();
            }

            foreach (var system in endInitEntitySystems)
            {
                system.OnEndInit(activeEntitiesPerSystem[system]);
            }

            foreach (var system in endInitSystems)
            {
                system.OnEndInit();
            }
        }

        public void Update()
        {
            if (paused)
            {
                return;
            }

            foreach (var system in frameEntitySystems)
            {
                system.OnFrame(activeEntitiesPerSystem[system]);
            }
            foreach (var system in updateSytems)
            {
                system.OnFrame();
            }
            //DeleteMarkedEntities();
        }

        public void FixedUpdate()
        {
            if (paused)
            {
                return;
            }

            foreach (var system in physicsFrameEntitySystems)
            {
                system.OnPhysicsFrame(activeEntitiesPerSystem[system]);
            }
        }

        public void Tick()
        {
            if (paused)
            {
                return;
            }

            foreach (var system in tickEntitySystems)
            {
                system.Tick(activeEntitiesPerSystem[system]);
            }
            foreach (var system in tickSystems)
            {
                system.Tick();
            }
            DeleteMarkedEntities();
        }

        public void Pause()
        {
            paused = true;

            foreach (var system in pausableSystems)
            {
                system.Pause(activeEntitiesPerSystem[system]);
            }
        }

        public void Resume()
        {
            foreach (var system in pausableSystems)
            {
                system.Resume(activeEntitiesPerSystem[system]);
            }

            paused = false;
        }

        public Entity CreateEntity(List<IState> states, bool copyStates = true, bool fireEntityAdded = true)
        {
            var newStates = copyStates ? states.DeepClone() : states;
            var entity = entityManager.BuildEntity(newStates);
            if (fireEntityAdded)
            {
                EntityAdded(entity);
            }
            return entity;
        }

        public Entity GetEntity(int entityId)
        {
            if (entityId == EntityIdComponent.InvalidEntityId)
            {
                UnityEngine.Debug.LogError("Tried to get entity using the invalid entity id.");
            }
            return entityManager.GetEntity(entityId);
        }

        public IEnumerable<Entity> GetEntitiesWithState<T>() where T : IState
        {
            return entityManager.GetEntitiesWithState<T>();
        }

        public void RemoveEntity(Entity entityToRemove)
        {
            if (entityToRemove != null)
            {
                entitiesToRemove.Add(entityToRemove);
            }
        }

        public string DebugEntity(int entityId)
        {
            var entity = entityManager.GetEntity(entityId);
            return entity.ToString();
        }

        private void DeleteMarkedEntities()
        {
            entitiesToRemove.ForEach(entityToRemove =>
            {
                EntityRemoved(entityToRemove);
                entityManager.DeleteEntity(entityToRemove);
                entitiesToRemove.Remove(entityToRemove);
            });
        }

        public void EntityAdded(Entity entity)
        {
            foreach (var system in activeEntitiesPerSystem.Keys)
            {
                //TODO: Cache required states to avoid unneeded alocation.
                var entityHasAllStates = system.RequiredStates().TrueForAll(entity.HasState);
                if (entityHasAllStates)
                {
                    activeEntitiesPerSystem[system].Add(entity);
                    if (system is IReactiveEntitySystem)
                    {
                        var reactiveSystem = system as IReactiveEntitySystem;
                        reactiveSystem.OnEntityAdded(entity);
                    }
                }
            }
        }

        private void EntityRemoved(Entity entity)
        {
            foreach (var system in activeEntitiesPerSystem.Keys)
            {
                //TODO: Cache required states to avoid unneeded alocation.
                var entityHasAllStates = system.RequiredStates().TrueForAll(entity.HasState);
                if (entityHasAllStates)
                {
                    if (system is IReactiveEntitySystem)
                    {
                        var reactiveSystem = system as IReactiveEntitySystem;
                        reactiveSystem.OnEntityRemoved(entity);
                    }
                    activeEntitiesPerSystem[system].Remove(entity);
                }
            }
        }
    }
}
