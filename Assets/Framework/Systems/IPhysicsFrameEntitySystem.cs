using System.Collections.Generic;
using Assets.Framework.Entities;

namespace Assets.Framework.Systems
{
    public interface IPhysicsFrameEntitySystem : IFilteredSystem
    {
        void OnPhysicsFrame(List<Entity> matchingEntities);
    }
}
