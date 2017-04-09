using Assets.Framework.Entities;
using Assets.Framework.Systems;
using System.Collections.Generic;

public interface IPausableSystem : IFilteredSystem
{
    void Pause(List<Entity> matchingEntities);
    void Resume(List<Entity> matchingEntities);
}
