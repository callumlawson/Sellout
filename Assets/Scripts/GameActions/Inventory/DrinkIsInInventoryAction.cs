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
        private readonly DrinkState drinkToCheckFor;

        private readonly int timeoutInMins;
        private GameTime startTime;
        private TimeState timeState;

        public DrinkIsInInventoryAction(DrinkState drinkToCheckFor, int timeoutInMins)
        {
            this.drinkToCheckFor = drinkToCheckFor;
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
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                ActionStatus = DrinkUtil.GetDifference(inventoryItem.GetState<DrinkState>(), drinkToCheckFor) == 0.0f ? ActionStatus.Succeeded : ActionStatus.Failed;
            }
            if (timeState.GameTime - startTime > timeoutInMins)
            {
                UnityEngine.Debug.Log("Timedout!");
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
