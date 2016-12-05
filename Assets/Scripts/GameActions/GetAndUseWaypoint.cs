using Assets.Framework.Entities;
using Assets.Scripts.GameActions.Framework;
using Assets.Scripts.States;
using Assets.Scripts.Systems;
using Assets.Scripts.Util;

namespace Assets.Scripts.GameActions
{
    class GetAndUseWaypointAction : GameAction
    {
        private Goal goal;

        public GetAndUseWaypointAction(Goal waypointGoal)
        {
            goal = waypointGoal;
        }

        public override void OnStart(Entity entity)
        {
            var waypoint = WaypointSystem.Instance.GetAndUseWaypointThatSatisfiedGoal(goal, entity);
            if (waypoint != null)
            {
                entity.GetState<ActionBlackboardState>().TargetWaypoint = waypoint;
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
