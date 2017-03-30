using System.Collections.Generic;
using Assets.Framework.Entities;
using Assets.Scripts.States;

namespace Assets.Scripts.Util
{
    public static class EntityQueries
    {
        public static Entity GetNPCWithName(List<Entity> entities, string name)
        {
            return entities.Find(entity => entity.HasState<NameState>() && entity.GetState<NameState>().Name == name);
        }
    }
}
