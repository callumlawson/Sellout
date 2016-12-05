using Assets.Framework.Entities;

namespace Assets.Scripts.Visualizers
{
    public interface IEntityVisualizer
    {
        void OnStartRendering(Entity entity);
        void OnFrame();
        void OnStopRendering(Entity entity);
    }
}
