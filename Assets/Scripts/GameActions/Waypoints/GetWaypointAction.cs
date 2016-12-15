using Assets.Framework.Entities;
using Assets.Scripts.GameActions.Framework;
using Assets.Scripts.States;
using Assets.Scripts.Systems;
using Assets.Scripts.Util;

namespace Assets.Scripts.GameActions.Waypoints
{
    class GetWaypointAction : GameAction
    {
        private readonly Goal goal;
        private readonly bool reserve;
        private readonly bool closest;

        public GetWaypointAction(Goal waypointGoal, bool reserve = false, bool closest = false)
        {
            goal = waypointGoal;
            this.reserve = reserve;
            this.closest = closest;
        }

        public override void OnStart(Entity entity)
        {
            var waypoint = closest ? WaypointSystem.Instance.GetClosestFreeWaypointThatSatisfiesGoal(entity, goal) : WaypointSystem.Instance.GetFreeWaypointThatSatisfiesGoal(goal);

            if (waypoint != null && reserve)
            {
                waypoint.GetState<UserState>().Reserver = entity;
            }

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
