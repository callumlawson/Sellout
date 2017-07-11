using System.Collections.Generic;
using Assets.Framework.Entities;
using Assets.Scripts.States;
using Assets.Scripts.Util.NPC;

namespace Assets.Scripts.Util
{
    public static class EntityQueries
    {
        public static Entity GetEntityWithName(List<Entity> entities, NPCName name)
        {
            return entities.Find(entity => entity.HasState<NameState>() && entity.GetState<NameState>().Name == name.ToString());
        }

        public static List<Entity> GetNPCSWithName(List<Entity> entities, NPCName name)
        {
            return entities.FindAll(entity => entity.HasState<NameState>() && entity.GetState<NameState>().Name == name.ToString());
        }
    }
}
