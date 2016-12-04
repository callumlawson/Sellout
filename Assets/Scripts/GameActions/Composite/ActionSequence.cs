using System.Linq;
using System.Text;
using Assets.Framework.Entities;
using Assets.Scripts.GameActions.Framework;
using Assets.Scripts.States;
using UnityEngine;

namespace Assets.Scripts.GameActions.Composite
{
    //Run actions in sequence ignoring failure
    class ActionSequence : CompositeAction
    {
        private bool paused;

        public override void OnStart(Entity entity)
        {
        }

        public bool NonEmpty()
        {
            return Actions.Count > 0;
        }

        public override void Pause()
        {
            paused = true;
            if (Actions.Count > 0)
            {
                Actions[0].Pause();
            }
        }

        public override void Unpause()
        {
            paused = false;
            if (Actions.Count > 0)
            {
                Actions[0].Unpause();
            }
        }

        public override void OnFrame(Entity entity)
        {
            UpdatePauseState(entity);

            if (paused)
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

        private void UpdatePauseState(Entity entity)
        {
            var entityIsPaused = entity.GetState<ActionBlackboardState>().Paused;
            if (entityIsPaused && !paused)
            {
                Pause();
            }
            else if (!entityIsPaused && paused)
            {
                Unpause();
            }
        }

        private void StartFirstActionIfNotStarted(Entity entity)
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
            message.Append("Action Sequence - Status: " + ActionStatus + " \n");
            foreach (var action in Actions)
            {
                message.AppendLine(action.ToString());
            }
            return message.ToString();
        }
    }
}
