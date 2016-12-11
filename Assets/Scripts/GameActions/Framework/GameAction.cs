using System;
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

    public static class ActionStatusExtensions
    {
        public static string ToColoredString(this ActionStatus actionStatus)
        {
            switch (actionStatus)
            {
                case ActionStatus.NotStarted:
                    return "Not Started";
                case ActionStatus.Running:
                    return "<color=yellow>Running</color>";
                case ActionStatus.Failed:
                    return "<color=red>Failed</color>";
                case ActionStatus.Succeeded:
                    return "<color=green>Succeeded</color>";
                default:
                    throw new ArgumentOutOfRangeException("actionStatus", actionStatus, null);
            }
        }
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
            return GetType().Name + " Status: " + ActionStatus.ToColoredString();
        }
    }

    public interface ICancellableAction
    {
        void Cancel();
        bool IsCancellable();
    }
}
