using Assets.Framework.Entities;

namespace Assets.Framework.Systems
{
    public interface IReactiveEntitySystem : IFilteredSystem
    {
        void OnEntityAdded(Entity entity);
        void OnEntityRemoved(Entity entity);
    }
}
