using Assets.Framework.Entities;
using Assets.Framework.Systems;
using Assets.Scripts.GameActions.Framework;
using Assets.Scripts.States;

namespace Assets.Scripts.GameActions.Inventory
{
    public class DestoryEntityInInventoryAction : GameAction
    {
        public override void OnStart(Entity entity)
        {
            var inventoryItem = entity.GetState<InventoryState>().Child;
            if (inventoryItem != null)
            {
                entity.GetState<InventoryState>().RemoveChild();
                EntityStateSystem.Instance.RemoveEntity(inventoryItem);
            }
            ActionStatus = ActionStatus.Succeeded;
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
