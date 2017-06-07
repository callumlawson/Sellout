using System;
using Assets.Framework.Entities;
using Assets.Framework.States;
using Assets.Scripts.GameActions.Framework;
using Assets.Scripts.States;
using Assets.Scripts.Systems;
using Assets.Scripts.Util;
using UnityEngine;

namespace Assets.Scripts.GameActions.Waypoints
{
    class GetWaypointAction : GameAction
    {
        private readonly Goal goal;
        private readonly bool reserve;
        private readonly bool closest;

        private TimeState timeState;
        private readonly int timeoutInMins;
        private GameTime startTime;

        public GetWaypointAction(Goal waypointGoal, bool reserve = false, bool closest = false, int timeoutInMins = 30)
        {
            goal = waypointGoal;
            this.reserve = reserve;
            this.closest = closest;
            this.timeoutInMins = timeoutInMins;
        }

        public override void OnStart(Entity entity)
        {
            timeState = StaticStates.Get<TimeState>();
            startTime = timeState.gameTime.GetCopy();
        }

        public override void OnFrame(Entity entity)
        {
            var waypoint = closest ? WaypointSystem.Instance.GetClosestFreeWaypointThatSatisfiesGoal(entity, goal) : WaypointSystem.Instance.GetFreeWaypointThatSatisfiesGoal(goal);

            if (waypoint != null && reserve)
            {
                waypoint.GetState<UserState>().Reserve(entity, "Get waypoint action");
            }

            if (waypoint != null)
            {
                entity.GetState<ActionBlackboardState>().TargetEntity = waypoint;
                ActionStatus = ActionStatus.Succeeded;
            }
            else if (timeoutInMins > 0 && (timeState.gameTime - startTime) > timeoutInMins)
            {
                ActionStatus = ActionStatus.Failed;
            }
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
