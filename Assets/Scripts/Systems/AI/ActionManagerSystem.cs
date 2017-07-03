using System.Collections.Generic;
using Assets.Framework.Entities;
using Assets.Framework.Systems;
using Assets.Scripts.GameActions.Composite;
using Assets.Scripts.GameActions.Framework;
using Assets.Scripts.States;
using Assets.Scripts.Util;
using UnityEngine.Profiling;

namespace Assets.Scripts.Systems.AI
{
    class ActionManagerSystem : IFrameSystem, IInitSystem
    {
        public static ActionManagerSystem Instance;

        private readonly Dictionary<Entity, ActionSequence> entityActions = new Dictionary<Entity, ActionSequence>();

        public void OnInit()
        {
            Instance = this;
            foreach (var entityToActions in entityActions)
            {
                entityToActions.Value.OnStart(entityToActions.Key);
            }
        }

        public void OnFrame()
        {
            Profiler.BeginSample("ActionManagerSystem-OnFrame");

            foreach (var entityToActions in entityActions)
            {
                entityToActions.Value.OnFrame(entityToActions.Key);
                Profiler.BeginSample("ActionManagerSystem-OnFrame-Debug");
                if (GameSettings.IsDebugOn)
                {
                    entityToActions.Key.GetState<ActionBlackboardState>().CurrentActions = entityToActions.Value.ToString();
                }
                Profiler.EndSample();
            }
            Profiler.EndSample();
        }

        public void QueueAction(Entity entity, GameAction action)
        {
            InitActionsIfNone(entity);
            entityActions[entity].Add(action);
        }

        public void AddActionToFrontOfQueueForEntity(Entity entity, GameAction action)
        {
            InitActionsIfNone(entity);
            entityActions[entity].AddToStart(action);
        }

        public void TryClearActionsForEntity(Entity entity)
        {
            if (entityActions.ContainsKey(entity))
            {
                var action = entityActions[entity];
                action.Cancel();
            }
        }

        public void TryCancelThenHardClearActions(Entity entity)
        {
            TryClearActionsForEntity(entity);
            if(entityActions.ContainsKey(entity))
            {
                entityActions[entity] = new ActionSequence("Root");
            }
        }

        public bool IsEntityIdle(Entity entity)
        {
            return !entityActions.ContainsKey(entity) || GetActionsForEntity(entity).IsComplete();
        }

        private void InitActionsIfNone(Entity entity)
        {
            if (!entityActions.ContainsKey(entity))
            {
                entityActions.Add(entity, new ActionSequence("Root"));
            }
        }

        private ActionSequence GetActionsForEntity(Entity entity)
        {
            InitActionsIfNone(entity);
            return entityActions[entity];
        }
    }
}
