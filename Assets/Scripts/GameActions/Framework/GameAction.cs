using Assets.Framework.Entities;
using JetBrains.Annotations;

namespace Assets.Scripts.GameActions.Framework
{
    public enum ActionStatus
    {
        NotStarted,
        Running,
        Failed,
        Succeeded
    }

    public abstract class GameAction
    {
        public ActionStatus ActionStatus = ActionStatus.NotStarted;

        public abstract void OnStart(Entity entity);
        public abstract void OnFrame(Entity entity);
        
        public bool IsStarted()
        {
            return ActionStatus == ActionStatus.Running || IsComplete();
        }

        public bool IsComplete()
        {
            return ActionStatus == ActionStatus.Failed || ActionStatus == ActionStatus.Succeeded;
        }
        
        public abstract void Pause();

        public abstract void Unpause();

        public override string ToString()
        {
            return GetType().Name + " Status: " + ActionStatus + " ";
        }
    }

    public interface ICancellableAction
    {
        void Cancel();
        bool IsCancellable();
    }
}
