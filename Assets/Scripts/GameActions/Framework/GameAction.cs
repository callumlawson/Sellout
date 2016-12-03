using Assets.Framework.Entities;

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

        public override string ToString()
        {
            return GetType().Name + " Status: " + ActionStatus + " ";
        }
    }
}
