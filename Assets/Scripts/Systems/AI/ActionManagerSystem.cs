using System.Collections.Generic;
using Assets.Framework.Entities;
using Assets.Framework.Systems;
using Assets.Scripts.GameActions.Composite;
using Assets.Scripts.GameActions.Framework;
using Assets.Scripts.States;

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
            foreach (var entityToActions in entityActions)
            {
                entityToActions.Value.OnFrame(entityToActions.Key);
                entityToActions.Key.GetState<ActionBlackboardState>().CurrentActions = entityToActions.Value.ToString();
            }
        }

        public void QueueActionForEntity(Entity entity, GameAction action)
        {
            if (!entityActions.ContainsKey(entity))
            {
                entityActions.Add(entity, new ActionSequence());
            }
            entityActions[entity].Add(action);
        }

        public ActionSequence GetActionsForEntity(Entity entity)
        {
            if (!entityActions.ContainsKey(entity))
            {
                return new ActionSequence();
            }

            return entityActions[entity];
        }

        public bool EntityHasActions(Entity entity)
        {
            if (!entityActions.ContainsKey(entity))
            {
                return false;
            }

            return entityActions[entity].NonEmpty();
        }

        public bool IsEntityIdle(Entity entity)
        {
            return !entityActions.ContainsKey(entity) || GetActionsForEntity(entity).IsComplete();
        }
    }
}
