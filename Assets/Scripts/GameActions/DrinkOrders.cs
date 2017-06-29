using System;
using Assets.Framework.Entities;
using Assets.Scripts.GameActions.Composite;
using Assets.Scripts.GameActions.Decorators;
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
            NonAlcoholic,
            ContainingIngredient
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

            public override string ToString()
            {
                return Recipe.DrinkName;
            }
        }

        public class NonAlcoholicDrinkOrder : DrinkOrder
        {
            public NonAlcoholicDrinkOrder(string ordererName) : base(DrinkOrderType.NonAlcoholic)
            {
                OrdererName = ordererName;
                DrinkPredicate = testDrink => DrinkState.IsNonAlcoholic(testDrink) && DrinkRecipes.Contains(testDrink);
            }

            public override string ToString()
            {
                return "Non Alcoholic";
            }
        }

        public class IncludingIngredientOrder : DrinkOrder
        {
            public readonly Ingredient Ingredient;

            public IncludingIngredientOrder(Ingredient ingredient, string ordererName) : base(DrinkOrderType.ContainingIngredient)
            {
                Ingredient = ingredient;
                OrdererName = ordererName;
                DrinkPredicate = testDrink => testDrink.ContainsIngedient(Ingredient) && DrinkRecipes.Contains(testDrink);
            }

            public override string ToString()
            {
                return "With " + Ingredient;
            }
        }

        public static GameAction GetRandomOrder(Entity entity, int orderTimeOurInMins = 20)
        {
            var randomValue = Random.value;
            if (randomValue <= 0.25)
            {
                var drinkOrder = new ExactDrinkorder(DrinkRecipes.GetRandomDrinkRecipe(), entity.GetState<NameState>().Name);
                return OrderDrink(entity, drinkOrder, new OrderExactDrinkConverstation(drinkOrder.Recipe.DrinkName), orderTimeOurInMins);
            }
            if (randomValue <= 0.50)
            {
                return OrderDrink(entity, new NonAlcoholicDrinkOrder(entity.GetState<NameState>().Name), new OrderNonAlcoholicDrinkConversation(), orderTimeOurInMins);
            }
            if (randomValue <= 0.75)
            {
                var ingredient = Ingredients.DispensedNonAlcoholicIngredients.PickRandom();
                return OrderDrink(entity, new IncludingIngredientOrder(ingredient, entity.GetState<NameState>().Name) , new OrderDrinkIncludingIngredientConversation(ingredient) , orderTimeOurInMins);
            }
            return OrderDrink(entity, new ExactDrinkorder(DrinkRecipes.Beer, entity.GetState<NameState>().Name), new OrderExactDrinkConverstation("Beer"), orderTimeOurInMins);
        }

        public static GameAction GetRandomAlcoholicDrinkOrder(Entity entity, int orderTimeOurInMins = 20)
        {
            var drinkOrder = new ExactDrinkorder(DrinkRecipes.GetRandomAlcoholicDrinkRecipe(), entity.GetState<NameState>().Name);
            return OrderDrink(entity, drinkOrder, new OrderExactDrinkConverstation(drinkOrder.Recipe.DrinkName), orderTimeOurInMins);
        }

        public static GameAction GetRandomAlcoholicDrinkOrderWithoutFailure(Entity entity, int orderTimeOurInMins = 20)
        {
            var drinkOrder = new ExactDrinkorder(DrinkRecipes.GetRandomAlcoholicDrinkRecipe(), entity.GetState<NameState>().Name);
            return OrderDrinkWithoutFailure(entity, drinkOrder, new OrderExactDrinkConverstation(drinkOrder.Recipe.DrinkName), orderTimeOurInMins);
        }

        public static ActionSequence OrderDrink(Entity entity, DrinkOrder drinkOrder, Conversation conversation, int orderTimeoutInMins = 20)
        {
            var wrapper = new ActionSequence("DrinkOrderThenClear");
            var orderDrink = new ParallelUntilAllCompleteAction("OrderDrink");
            orderDrink.Add(new ReportSuccessDecorator(new ConversationAction(conversation)));
            var waitForDrink = new ConditionalActionSequence("WaitForDrink");
            waitForDrink.Add(new StartDrinkOrderAction(drinkOrder));
            waitForDrink.Add(CommonActions.WaitForDrink(entity, drinkOrder.DrinkPredicate, orderTimeoutInMins));
            orderDrink.Add(waitForDrink);
            wrapper.Add(orderDrink);
            wrapper.Add(new ClearConversationAction());
            return wrapper;
        }

        public static GameAction OrderDrinkWithoutFailure(Entity entity, DrinkOrder drinkOrder, Conversation conversation, int orderTimeoutInMins = 20)
        {
            var orderDrink = new ParallelUntilAllCompleteAction("OrderDrink");
            orderDrink.Add(new ReportSuccessDecorator(new ConversationAction(conversation)));
            var waitForDrink = new ConditionalActionSequence("WaitForDrink");
            waitForDrink.Add(new StartDrinkOrderAction(drinkOrder));
            waitForDrink.Add(CommonActions.WaitForDrinkWithoutFailure(entity, drinkOrder.DrinkPredicate, orderTimeoutInMins));
            orderDrink.Add(waitForDrink);
            return orderDrink;
        }

        public class OrderExactDrinkConverstation : Conversation
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
            }
        }

        private class OrderNonAlcoholicDrinkConversation : Conversation
        {
            protected override void StartConversation(string converstationInitiator)
            {
                DialogueSystem.Instance.StartDialogue(converstationInitiator);
                DialogueSystem.Instance.WriteNPCLine("I'll have anything on the menu, as long as it's non-alcoholic.");
                DialogueSystem.Instance.WriteNPCLine(Random.value > 0.5 ? "...had a heavy one last night.": "Got a shift starting soon.");
            }
        }

        private class OrderDrinkIncludingIngredientConversation : Conversation
        {
            private readonly Ingredient ingredient;

            public OrderDrinkIncludingIngredientConversation(Ingredient ingredient)
            {
                this.ingredient = ingredient;
            }

            protected override void StartConversation(string converstationInitiator)
            {
                DialogueSystem.Instance.StartDialogue(converstationInitiator);
                DialogueSystem.Instance.WriteNPCLine("Give me something containing " + ingredient + ". It's my favourite");
            }
        }
    }
}
