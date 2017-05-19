using System.Collections.Generic;
using System.Linq;
using Assets.Framework.Entities;
using Assets.Scripts.GameActions.Framework;

namespace Assets.Scripts.GameActions
{
    class SyncedAction : GameAction
    {
        private readonly Dictionary<Entity, bool> entityToReady = new Dictionary<Entity, bool>();

        public SyncedAction(Entity one, Entity two)
        {
            entityToReady.Add(one, false);
            entityToReady.Add(two, false);
        }

        public SyncedAction(List<Entity> entities)
        {
            entities.ForEach(entity => entityToReady.Add(entity, false));
        }

        public override void OnFrame(Entity entity)
        {
//            UnityEngine.Debug.Log("Entity: " + entity + " on Frame");
        }

        public override void OnStart(Entity entity)
        {
            UnityEngine.Debug.Log("Entity: " + entity + " " + entityToReady.Count);

            if (!entityToReady.ContainsKey(entity))
            {
                UnityEngine.Debug.LogError("Synced action started by entity: " + entity +  " not in synced action.");
            }
            else
            {
                entityToReady[entity] = true;
            }

            if (entityToReady.All(keyvalue => keyvalue.Value))
            {
                UnityEngine.Debug.Log("Synced action done");
                ActionStatus = ActionStatus.Succeeded;
            }
        }

        public override void Pause()
        {
            //Do nothing
        }

        public override void Unpause()
        {
            //Do nothing
        }
    }
}
