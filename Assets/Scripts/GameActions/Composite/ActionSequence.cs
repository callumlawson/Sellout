using System.Linq;
using System.Text;
using Assets.Framework.Entities;
using Assets.Scripts.GameActions.Framework;
using Assets.Scripts.States;

namespace Assets.Scripts.GameActions.Composite
{
    abstract class ActionSequenceBase : CompositeAction, ICancellableAction
    {
        private bool SequenceIsCancellable;

        public ActionSequenceBase(string name = "Unnamed", bool isCancellable = true)
        {
            CompositeActionName = name;
            SequenceIsCancellable = isCancellable;
        }

        protected bool Paused;

        public override void Pause()
        {
            Paused = true;
            if (Actions.Count > 0)
            {
                Actions[0].Pause();
            }
        }

        public override void Unpause()
        {
            Paused = false;
            if (Actions.Count > 0)
            {
                Actions[0].Unpause();
            }
        }

        protected void UpdatePauseState(Entity entity)
        {
            var entityIsPaused = entity.GetState<ActionBlackboardState>().Paused;
            if (entityIsPaused && !Paused)
            {
                Pause();
            }
            else if (!entityIsPaused && Paused)
            {
                Unpause();
            }
        }

        protected void StartFirstActionIfNotStarted(Entity entity)
        {
            if (!Actions[0].IsStarted())
            {
                Actions[0].ActionStatus = ActionStatus.Running;
                Actions[0].OnStart(entity);
            }
        }

        public override string ToString()
        {
            var message = new StringBuilder();
            message.Append("Action Sequence: " + CompositeActionName + " - Status: " + ActionStatus.ToColoredString());
            foreach (var action in Actions)
            {
                message.Append("\n" + action);
            }
            if (Actions.Count == 0)
            {
                message.AppendLine();
            }
            return message.ToString();
        }

        public void Cancel()
        {
            if (IsCancellable())
            {
                Actions.ForEach(action =>
                {
                    var cancellableAction = action as ICancellableAction;
                    if (cancellableAction != null) cancellableAction.Cancel();
                });
                Actions.Clear();
                ActionStatus = ActionStatus.Succeeded;
            }
        }
        
        public bool IsCancellable()
        {
            if (!SequenceIsCancellable)
            {
                return false;
            }

            if (Actions.Count == 0)
            {
                return true;
            }

            var cancellable = Actions[0] as ICancellableAction;
            return cancellable != null && cancellable.IsCancellable();
        }
    }

    //Run actions in sequence. Failure prevent remaining actions being run.
    class ConditionalActionSequence : ActionSequenceBase
    {
        public ConditionalActionSequence(string name = "Unnamed", bool isCancellable = true) : base(name, isCancellable)
        {
        }

        public override void OnStart(Entity entity)
        {
            //Do Nothing
        }

        public override void OnFrame(Entity entity)
        {
            UpdatePauseState(entity);

            if (Paused)
            {
                return;
            }

            if (Actions.Any())
            {
                ActionStatus = ActionStatus.Running;

                StartFirstActionIfNotStarted(entity);

                Actions[0].OnFrame(entity);

                if (Actions[0].IsComplete())
                {
                    var completionStatus = Actions[0].ActionStatus;

                    if (completionStatus == ActionStatus.Succeeded)
                    {
                        Actions.RemoveAt(0);

                        if (Actions.Any())
                        {
                            Actions[0].OnStart(entity);
                        }
                    }
                    else if (completionStatus == ActionStatus.Failed)
                    {
                        Actions.Clear();
                        ActionStatus = ActionStatus.Failed;
                    }
                }
            }
            else if(!IsComplete())
            {
                ActionStatus = ActionStatus.Succeeded;
            }
        }
    }

    //Run actions in sequence. Continues after failure.
    class ActionSequence : ActionSequenceBase
    {
        public ActionSequence(string name = "Unnamed", bool isCancellable = true) : base(name, isCancellable)
        {
        }

        public override void OnStart(Entity entity)
        {
            //Do Nothing
        }

        public override void OnFrame(Entity entity)
        {
            UpdatePauseState(entity);

            if (Paused)
            {
                return;
            }

            if (Actions.Any())
            {
                ActionStatus = ActionStatus.Running;

                StartFirstActionIfNotStarted(entity);

                Actions[0].OnFrame(entity);

                if (Actions[0].IsComplete())
                {
                    Actions.RemoveAt(0);

                    if (Actions.Any())
                    {
                        Actions[0].OnStart(entity);
                    }
                }
            }
            else
            {
                ActionStatus = ActionStatus.Succeeded;
            }
        }
    }
}
