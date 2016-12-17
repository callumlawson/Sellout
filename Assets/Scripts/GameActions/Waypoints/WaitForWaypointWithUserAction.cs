using System;
using Assets.Framework.Entities;
using Assets.Framework.States;
using Assets.Scripts.GameActions.Framework;
using Assets.Scripts.States;
using Assets.Scripts.Systems;
using Assets.Scripts.Util;

namespace Assets.Scripts.GameActions.Waypoints
{
    class WaitForWaypointWithUserAction : GameAction
    {
        private readonly Goal goal;
        private readonly Entity occupant;
        private readonly int timeoutInMins;
        private DateTime startTime;
        private TimeState timeState;

        public WaitForWaypointWithUserAction(Goal goal, Entity occupant, int timeoutInMins)
        {
            this.goal = goal;
            this.occupant = occupant;
            this.timeoutInMins = timeoutInMins;
        }

        public override void OnStart(Entity entity)
        {
            timeState = StaticStates.Get<TimeState>();
            startTime = timeState.time;
        }

        public override void OnFrame(Entity entity)
        {
            TryGetWaypoint(entity);
        }

        public override void Pause()
        {
            //Do Nothing
        }

        public override void Unpause()
        {
            //Do Nothing
        }

        private void TryGetWaypoint(Entity entity)
        {
            var waypoint = WaypointSystem.Instance.GetWaypointThatSatisfiesGoalWithOcupant(goal, occupant);
            if (waypoint != null)
            {
                ActionStatus = ActionStatus.Succeeded;
            }
            if ((timeState.time - startTime).Duration().Minutes > timeoutInMins)
            {
                ActionStatus = ActionStatus.Failed;
            }
        }
    }
}
