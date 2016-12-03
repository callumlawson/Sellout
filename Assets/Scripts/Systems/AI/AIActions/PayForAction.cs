using Assets.Framework.Entities;
using Assets.Framework.States;
using Assets.Scripts.States;
using Assets.Scripts.Util;

namespace Assets.Scripts.Systems.AI.AIActions
{
    class PayForAction : ActionSequence
    {
        public override void OnStart(Entity entity)
        {
            var waypoint = WaypointSystem.Instance.GetFreeWaypointThatSatisfiesGoal(Goal.PayFor);
            if (waypoint != null)
            {
                Add(new GoToPositionAction(waypoint.GetState<PositionState>().Position));
                Add(new DestoryEntityInInventoryAction(entity.GetState<InventoryState>().child));
            }
            else
            {
                IsComplete = true;
            }
        }
    }
}
