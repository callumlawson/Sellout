using System;
using Assets.Framework.Entities;
using Assets.Framework.States;
using Assets.Scripts.GameActions.Framework;
using Assets.Scripts.States;
using Assets.Scripts.Util;
using Assets.Scripts.Util.Dialogue;

namespace Assets.Scripts.GameActions.Inventory
{
    public class DrinkIsInInventoryAction : GameAction
    {
        private readonly int timeoutInMins;
        private GameTime startTime;
        private TimeState timeState;

        private DrinkOrders.DrinkOrder drinkOrder;

        public DrinkIsInInventoryAction(DrinkOrders.DrinkOrder drinkOrder, int timeoutInMins)
        {
            this.drinkOrder = drinkOrder;
            this.timeoutInMins = timeoutInMins;
        }

        public override void OnStart(Entity entity)
        {
            timeState = StaticStates.Get<TimeState>();
            startTime = timeState.GameTime.GetCopy();
        }

        public override void OnFrame(Entity entity)
        {
            var inventoryItem = entity.GetState<InventoryState>().Child;
            if (inventoryItem != null && inventoryItem.HasState<DrinkState>())
            {
                IncorrectDrinkReason reason;
                var success = drinkOrder.IsValidForOrder(inventoryItem.GetState<DrinkState>(), out reason);

                entity.GetState<ActionBlackboardState>().IncorrectDrinkReason = reason;
                ActionStatus = success ? ActionStatus.Succeeded : ActionStatus.Failed;
                
            }
            if (timeState.GameTime - startTime > timeoutInMins)
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
