using System.Collections.Generic;
using Assets.Framework.Entities;

namespace Assets.Framework.Systems
{
    public interface IUpdateEntitySystem : IFilteredSystem
    {
        void Update(List<Entity> matchingEntities);
    }
}
