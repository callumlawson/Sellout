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
        private readonly DrinkState drinkState;

        private readonly int timeoutInMins;
        private DateTime startTime;
        private TimeState timeState;

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
            var inventoryItem = entity.GetState<HierarchyState>().Child;
            if (inventoryItem != null && inventoryItem.HasState<DrinkState>())
            {
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                ActionStatus = DrinkUtil.GetDifference(inventoryItem.GetState<DrinkState>(), drinkState) == 0.0f ? ActionStatus.Succeeded : ActionStatus.Failed;
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
