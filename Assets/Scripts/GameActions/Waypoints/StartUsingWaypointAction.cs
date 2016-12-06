using Assets.Framework.Entities;
using Assets.Scripts.GameActions.Framework;
using Assets.Scripts.States;
using Assets.Scripts.Systems;

namespace Assets.Scripts.GameActions.Waypoints
{
    class StartUsingWaypointAction : GameAction
    {
        public override void OnStart(Entity entity)
        {
            var targetWaypoint = entity.GetState<ActionBlackboardState>().TargetEntity;
            WaypointSystem.StartUsingWaypoint(targetWaypoint, entity);
            ActionStatus = ActionStatus.Succeeded;
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
