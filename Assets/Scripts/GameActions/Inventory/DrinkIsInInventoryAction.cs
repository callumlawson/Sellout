using System;
using Assets.Framework.Entities;
using Assets.Framework.States;
using Assets.Scripts.GameActions.Framework;
using Assets.Scripts.States;

namespace Assets.Scripts.GameActions.Inventory
{
    public class DrinkIsInInventoryAction : GameAction
    {
        private DrinkState drinkState;
        private DateTime startTime;
        private TimeState timeState;
        private int timeoutInMins;

        public DrinkIsInInventoryAction(DrinkState drinkState, int timeoutInMins)
        {
            this.drinkState = drinkState;
            this.timeoutInMins = timeoutInMins;
        }

        public override void OnStart(Entity entity)
        {
            timeState = StaticStates.Get<TimeState>();
            startTime = timeState.time;
        }

        public override void OnFrame(Entity entity)
        {
            var inventoryItem = entity.GetState<InventoryState>().child;
            if (inventoryItem != null && inventoryItem.HasState<DrinkState>())
            {
                if (inventoryItem.GetState<DrinkState>().ToString() == drinkState.ToString()) //Horrible hack. Sorry.
                {
                    ActionStatus = ActionStatus.Succeeded;
                }
                else
                {
                    ActionStatus = ActionStatus.Failed;
                }
            }
            if ((timeState.time - startTime).Duration().Minutes > timeoutInMins)
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
