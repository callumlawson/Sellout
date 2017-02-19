using Assets.Framework.Entities;
using Assets.Scripts.GameActions.Framework;
using Assets.Scripts.States;
using Assets.Scripts.Systems;

namespace Assets.Scripts.GameActions.Inventory
{
    class PickUpItem : GameAction
    {
        public override void OnStart(Entity entity)
        {
            var inventoryItem = entity.GetState<InventoryState>().Child;
            var targetEntity = entity.GetState<ActionBlackboardState>().TargetEntity;

            if (inventoryItem == null && targetEntity != null)
            {
                FreeParentIfUsingIt(targetEntity);
                EventSystem.ParentingRequestEvent.Invoke(new ParentingRequest {EntityFrom = null, EntityTo = entity, Mover = targetEntity });
                ActionStatus = ActionStatus.Succeeded;
                return;
            }
            ActionStatus = ActionStatus.Failed;
        }

        private static void FreeParentIfUsingIt(Entity targetEntity)
        {
            var targetHierarchyState = targetEntity.GetState<InventoryState>();
            if (targetHierarchyState.Parent != null)
            {
                if (targetHierarchyState.Parent.HasState<UserState>())
                {
                    var parentUserState = targetHierarchyState.Parent.GetState<UserState>();
                    if (Equals(parentUserState.User, targetEntity))
                    {
                        parentUserState.ClearUser();
                    }
                    if (Equals(parentUserState.Reserver, targetEntity))
                    {
                        parentUserState.ClearReserver();
                    }
                }
            }
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
