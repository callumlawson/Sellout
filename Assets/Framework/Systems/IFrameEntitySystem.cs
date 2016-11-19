using System.Collections.Generic;
using Assets.Framework.Entities;

namespace Assets.Framework.Systems
{
    public interface IFrameEntitySystem : IFilteredSystem
    {
        void OnFrame(List<Entity> matchingEntities);
    }
}
