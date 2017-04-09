using System.Collections.Generic;
using Assets.Framework.Entities;
using Assets.Scripts.States;

namespace Assets.Scripts.Util
{
    public static class EntityQueries
    {
        public static Entity GetNPC(List<Entity> entities, string name)
        {
            return entities.Find(entity => entity.HasState<NameState>() && entity.GetState<NameState>().Name == name);
        }

        public static List<Entity> GetNPCSWithName(List<Entity> entities, string name)
        {
            return entities.FindAll(entity => entity.HasState<NameState>() && entity.GetState<NameState>().Name == name);
        }
    }
}
