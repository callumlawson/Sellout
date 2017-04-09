using System.Collections.Generic;
using Assets.Framework.Entities;

namespace Assets.Framework.Systems
{
    public interface IEndInitEntitySystem : IFilteredSystem
    {
        void OnEndInit(List<Entity> matchingEntities);
    }
}
