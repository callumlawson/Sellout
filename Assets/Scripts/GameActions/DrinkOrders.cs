using System;
using Assets.Framework.Entities;
using Assets.Scripts.GameActions.Composite;
using Assets.Scripts.GameActions.Dialogue;
using Assets.Scripts.GameActions.Drinks;
using Assets.Scripts.GameActions.Framework;
using Assets.Scripts.States;
using Assets.Scripts.Systems;
using Assets.Scripts.Util;
using Assets.Scripts.Util.Dialogue;
using Random = UnityEngine.Random;

namespace Assets.Scripts.GameActions
{
    public static class DrinkOrders
    {
        public enum DrinkOrderType
        {
            Exact,
            NonAlcoholic
        }

        public abstract class DrinkOrder
        {
            public const string OrdererSpecies = "Human";
            public string OrdererName;
            public readonly DrinkOrderType OrderType;
            public Func<DrinkState, bool> DrinkPredicate;

            protected DrinkOrder(DrinkOrderType orderType)
            {
                OrderType = orderType;
            }
        }

        public class ExactDrinkorder : DrinkOrder
        {
            public readonly DrinkRecipe Recipe;

            public ExactDrinkorder(DrinkRecipe recipe, string ordererName) : base(DrinkOrderType.Exact)
            {
                Recipe = recipe;
                OrdererName = ordererName;
                DrinkPredicate = testDrink => DrinkState.IsIdentical(recipe.Contents, testDrink);
            }
        }

        public class NonAlcoholicDrinkOrder : DrinkOrder
        {
            public NonAlcoholicDrinkOrder(string ordererName) : base(DrinkOrderType.NonAlcoholic)
            {
                OrdererName = ordererName;
                DrinkPredicate = DrinkState.IsNonAlcoholic;
            }
        }

        public static GameAction GetRandomOrder(Entity entity, int orderTimeOurInMins = 20)
        {
            return Random.value > 0.5 
                ? OrderExactDrink(entity, new ExactDrinkorder(DrinkRecipes.GetRandomDrinkRecipe(), entity.GetState<NameState>().Name), orderTimeOurInMins) 
                : OrderNonAlcoholicDrink(entity, new NonAlcoholicDrinkOrder(entity.GetState<NameState>().Name), orderTimeOurInMins);
        }

        public static ConditionalActionSequence OrderExactDrink(Entity entity, ExactDrinkorder drinkOrder, int orderTimeoutInMins = 20)
        {
            var orderDrink = new ConditionalActionSequence("OrderExactDrink");
            orderDrink.Add(new ConversationAction(new OrderExactDrinkConverstation(drinkOrder.Recipe.DrinkName)));
            orderDrink.Add(new StartDrinkOrderAction(drinkOrder));
            orderDrink.Add(CommonActions.WaitForDrink(entity, drinkOrder.DrinkPredicate, orderTimeoutInMins));
            return orderDrink;
        }

        public static ConditionalActionSequence OrderNonAlcoholicDrink(Entity entity, NonAlcoholicDrinkOrder drinkOrder, int orderTimeoutInMins = 20)
        {
            var orderDrink = new ConditionalActionSequence("OrderNonAlcoholicDrink");
            orderDrink.Add(new ConversationAction(new OrderNonAlcoholicDrinkConverstation()));
            orderDrink.Add(new StartDrinkOrderAction(drinkOrder));
            orderDrink.Add(CommonActions.WaitForDrink(entity, drinkOrder.DrinkPredicate, orderTimeoutInMins));
            return orderDrink;
        }

        private class OrderExactDrinkConverstation : Conversation
        {
            private readonly string drinkName;

            public OrderExactDrinkConverstation(string drinkName)
            {
                this.drinkName = drinkName;
            }

            protected override void StartConversation(string converstationInitiator)
            {
                DialogueSystem.Instance.StartDialogue(converstationInitiator);
                DialogueSystem.Instance.WriteNPCLine("I'd like a " + drinkName + " please.");
                DialogueSystem.Instance.WritePlayerChoiceLine("<i>Nod.</i>", EndConversation(DialogueOutcome.Default));
            }
        }

        private class OrderNonAlcoholicDrinkConverstation : Conversation
        {
            protected override void StartConversation(string converstationInitiator)
            {
                DialogueSystem.Instance.StartDialogue(converstationInitiator);
                DialogueSystem.Instance.WriteNPCLine("I'll have anything on the menu, as long as it's non-alcoholic.");
                DialogueSystem.Instance.WriteNPCLine(Random.value > 0.5 ? "...had a heavy one last night.": "Got a shift starting soon.");
                DialogueSystem.Instance.WritePlayerChoiceLine("<i>No problem.</i>", EndConversation(DialogueOutcome.Default));
            }
        }
    }
}
