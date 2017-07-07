using Assets.Framework.Entities;
using Assets.Framework.States;
using Assets.Scripts.GameActions.Framework;
using Assets.Scripts.States;
using Assets.Scripts.Systems;

namespace Assets.Scripts.GameActions.DayPhases
{
    public class ChangeDayPhaseAction : GameAction
    {
        public override void OnStart(Entity entity)
        {
            var dayPhaseState = StaticStates.Get<DayPhaseState>();
            if (dayPhaseState.CurrentDayPhase != DayPhase.Open)
            {
                DayDirectorSystem.Instance.RequestIncrementDayPhase();
            }
            ActionStatus = ActionStatus.Succeeded;
        }

        public override void OnFrame(Entity entity)
        {
            //Do nothing;
        }

        public override void Pause()
        {
            //Do nothing;
        }

        public override void Unpause()
        {
            //Do nothing;
        }
    }
}
