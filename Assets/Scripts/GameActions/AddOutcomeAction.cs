using Assets.Framework.Entities;
using Assets.Framework.States;
using Assets.Scripts.GameActions.Framework;
using Assets.Scripts.States;

namespace Assets.Scripts.GameActions
{
    public class AddOutcomeAction : GameAction
    {
        private readonly string outcome;

        public AddOutcomeAction(string outcome)
        {
            this.outcome = outcome;
        }

        public override void OnFrame(Entity entity)
        {
            //Do Nothing
        }

        public override void OnStart(Entity entity)
        {
            StaticStates.Get<OutcomeTrackerState>().AddOutcome(outcome);
            ActionStatus = ActionStatus.Succeeded;
        }

        public override void Pause()
        {
            //Do Nothing
        }

        public override void Unpause()
        {
            //Do Nothing
        }
    }
}
