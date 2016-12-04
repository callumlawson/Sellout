using Assets.Framework.Entities;
using Assets.Scripts.GameActions.Framework;
using Assets.Scripts.States;
using Assets.Scripts.Systems;
using Assets.Scripts.Util;

namespace Assets.Scripts.GameActions
{
    class GetWaypointAction : GameAction
    {
        private Goal goal;

        public GetWaypointAction(Goal waypointGoal)
        {
            goal = waypointGoal;
        }

        public override void OnStart(Entity entity)
        {
            var waypoint = WaypointSystem.Instance.GetFreeWaypointThatSatisfiesGoal(goal);
            if (waypoint != null)
            {
                entity.GetState<ActionBlackboardState>().TargetEntity = waypoint;
                ActionStatus = ActionStatus.Succeeded;
            }
            else
            {
                ActionStatus = ActionStatus.Failed;
            }
        }

        public override void OnFrame(Entity entity)
        {
            //Do Nothing
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
