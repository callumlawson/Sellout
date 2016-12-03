using System.Collections.Generic;
using Assets.Framework.Entities;
using Assets.Framework.Systems;
using Assets.Scripts.Systems.AI.AIActions;
using UnityEditor;

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
            return entityActions[entity];
        }

        public bool IsEntityIdle(Entity entity)
        {
            return !entityActions.ContainsKey(entity) || GetActionsForEntity(entity).IsComplete;
        }
    }
}
