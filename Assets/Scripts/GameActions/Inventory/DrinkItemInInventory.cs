using Assets.Framework.Entities;
using Assets.Scripts.GameActions.Framework;
using Assets.Scripts.States;

namespace Assets.Scripts.GameActions.Inventory
{
    class DrinkItemInInventory : GameAction
    {
        public override void OnStart(Entity entity)
        {
            var inventoryItem = entity.GetState<InventoryState>().Child;
            if (inventoryItem != null)
            {
                var drinkState = inventoryItem.GetState<DrinkState>();
                drinkState.Clear();
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
