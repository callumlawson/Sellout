using Assets.Framework.Entities;
using Assets.Scripts.GameActions.Composite;
using Assets.Scripts.GameActions.Dialogue;
using Assets.Scripts.States;
using Assets.Scripts.Systems;
using Assets.Scripts.Util;
using Assets.Scripts.Util.Dialogue;

namespace Assets.Scripts.GameActions
{
    static class DrinkOrders
    {
        public static ConditionalActionSequence OrderExactDrink(Entity entity, DrinkRecipe drinkRecipe, int orderTimeoutInMins = 20)
        {
            var orderDrink = new ConditionalActionSequence("OrderExactDrink");
            orderDrink.Add(new ConversationAction(new Dialogues.OrderDrinkConverstation(drinkRecipe.DrinkName)));
            orderDrink.Add(new StartDrinkOrderAction(new DrinkOrder
            {
                OrdererName = entity.GetState<NameState>().Name,
                OrdererSpecies = "Human",
                Recipe = drinkRecipe
            }));
            orderDrink.Add(CommonActions.WaitForDrink(entity, drinkRecipe, orderTimeoutInMins));
            return orderDrink;
        }

        //TODO Add OrderNonAlcoholicDrink!
    }
}
