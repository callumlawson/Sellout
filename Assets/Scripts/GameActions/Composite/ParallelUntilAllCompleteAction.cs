using System.Linq;
using Assets.Framework.Entities;
using Assets.Scripts.GameActions.Framework;

namespace Assets.Scripts.GameActions.Composite
{
    //Not tested yet!
    public class ParallelUntilAllCompleteAction : CompositeAction, ICancellableAction
    {
        public ParallelUntilAllCompleteAction(string name = "Unnamed") 
        {
            CompositeActionName = name;
        }

        public override void OnStart(Entity entity)
        {
            Actions.ForEach(action =>
            {
                action.OnStart(entity);
                action.ActionStatus = ActionStatus.Running;
            });
        }

        public override void OnFrame(Entity entity)
        {
            Actions.ForEach(action => action.OnFrame(entity));
            if (Actions.All(action => action.IsComplete()))
            {
                ActionStatus = ActionStatus.Succeeded;
            }
        }

        public override void Pause()
        {
            //Do nothing.
        }

        public override void Unpause()
        {
            //Do nothing.
        }

        public void Cancel()
        {
            Actions.ForEach(action =>
            {
                var cancellableAction = action as ICancellableAction;
                if (cancellableAction != null && action.IsStarted()) cancellableAction.Cancel();
            });
        }

        public bool IsCancellable()
        {
            return true;
        }
    }
}
