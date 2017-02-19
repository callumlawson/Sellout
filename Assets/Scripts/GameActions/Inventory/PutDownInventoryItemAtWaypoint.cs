using Assets.Framework.Entities;
using Assets.Scripts.GameActions.Framework;
using Assets.Scripts.States;
using Assets.Scripts.Systems;
using Assets.Scripts.Util.Events;

namespace Assets.Scripts.GameActions.Inventory
{
    class PutDownInventoryItemAtWaypoint : GameAction
    {
        public override void OnStart(Entity entity)
        {
            var inventoryItem = entity.GetState<InventoryState>().Child;
            var waypointEntity = entity.GetState<ActionBlackboardState>().TargetEntity;
          
            if (inventoryItem != null && waypointEntity != null)
            {
                var waypointUserState = waypointEntity.GetState<UserState>();
                if (waypointUserState.IsFree())
                {
                    EventSystem.ParentingRequestEvent.Invoke(new ParentingRequest {EntityFrom = entity, EntityTo = waypointEntity , Mover = inventoryItem });
                    WaypointSystem.StartUsingWaypoint(waypointEntity, inventoryItem);
                    ActionStatus = ActionStatus.Succeeded;
                    return;
                }
            }
            ActionStatus = ActionStatus.Failed;
        }

        public override void OnFrame(Entity entity)
        {
            //Nothing doing.
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
