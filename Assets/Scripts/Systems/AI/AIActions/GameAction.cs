using Assets.Framework.Entities;

namespace Assets.Scripts.Systems.AI.AIActions
{
    public abstract class GameAction
    {
        public abstract void OnStart(Entity entity);
        public abstract void OnFrame(Entity entity);
        public bool IsComplete;
        public bool IsStarted;
    }
}
