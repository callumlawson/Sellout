using Assets.Framework.Entities;
using Assets.Scripts.GameActions.Framework;
using Assets.Scripts.Systems;

namespace Assets.Scripts.GameActions.Drinks
{
    public class StartDrinkOrderAction : GameAction
    {
        private readonly DrinkOrders.DrinkOrder drinkOrder;

        public StartDrinkOrderAction(DrinkOrders.DrinkOrder order)
        {
            drinkOrder = order;
        }

        public override void OnStart(Entity entity)
        {
            EventSystem.StartDrinkOrderEvent.Invoke(drinkOrder);
            ActionStatus = ActionStatus.Succeeded;
        }

        public override void OnFrame(Entity entity)
        {
            // do nothing
        }    

        public override void Pause()
        {
            // do nothing
        }

        public override void Unpause()
        {
            // do nothing
        }
    }
}
