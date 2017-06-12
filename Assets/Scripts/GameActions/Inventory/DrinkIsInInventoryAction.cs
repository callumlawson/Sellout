using System;
using Assets.Framework.Entities;
using Assets.Framework.States;
using Assets.Scripts.GameActions.Framework;
using Assets.Scripts.States;
using Assets.Scripts.Util;

namespace Assets.Scripts.GameActions.Inventory
{
    public class DrinkIsInInventoryAction : GameAction
    {
        private readonly int timeoutInMins;
        private GameTime startTime;
        private TimeState timeState;

        private Func<DrinkState, bool> drinkPredicate;

        public DrinkIsInInventoryAction(Func<DrinkState, bool> drinkPredicate, int timeoutInMins)
        {
            this.drinkPredicate = drinkPredicate;
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
                ActionStatus = drinkPredicate.Invoke(inventoryItem.GetState<DrinkState>()) ? ActionStatus.Succeeded : ActionStatus.Failed;
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
