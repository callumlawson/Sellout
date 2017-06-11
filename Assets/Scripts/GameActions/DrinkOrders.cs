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
        }

        public class NonAlcoholicDrinkOrder : DrinkOrder
        {
            public NonAlcoholicDrinkOrder(string ordererName) : base(DrinkOrderType.NonAlcoholic)
            {
                OrdererName = ordererName;
                DrinkPredicate = testDrink => DrinkState.IsNonAlcoholic(testDrink) && DrinkRecipes.Contains(testDrink);
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
        }

        public static GameAction GetRandomOrder(Entity entity, int orderTimeOurInMins = 20)
        {
            var randomValue = Random.value;
            if (randomValue <= 0.25)
            {
                return OrderExactDrink(entity, new ExactDrinkorder(DrinkRecipes.GetRandomDrinkRecipe(), entity.GetState<NameState>().Name), orderTimeOurInMins);
            }
            if (randomValue <= 0.50)
            {
                return OrderNonAlcoholicDrink(entity, new NonAlcoholicDrinkOrder(entity.GetState<NameState>().Name), orderTimeOurInMins);
            }
            if (randomValue <= 0.75)
            {
                return OrderSpecificIngredientDrink(entity, new IncludingIngredientOrder(Ingredients.DispensedNonAlcoholicIngredients.PickRandom(), entity.GetState<NameState>().Name));
            }
            return OrderExactDrink(entity, new ExactDrinkorder(DrinkRecipes.Beer, entity.GetState<NameState>().Name), orderTimeOurInMins);
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
            orderDrink.Add(new ConversationAction(new OrderNonAlcoholicDrinkConversation()));
            orderDrink.Add(new StartDrinkOrderAction(drinkOrder));
            orderDrink.Add(CommonActions.WaitForDrink(entity, drinkOrder.DrinkPredicate, orderTimeoutInMins));
            return orderDrink;
        }

        public static ConditionalActionSequence OrderSpecificIngredientDrink(Entity entity, IncludingIngredientOrder drinkOrder, int orderTimeoutInMins = 20)
        {
            var orderDrink = new ConditionalActionSequence("OrderSpecificIngredientDrink");
            orderDrink.Add(new ConversationAction(new OrderDrinkIncludingIngredientConversation(drinkOrder.Ingredient)));
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

        private class OrderNonAlcoholicDrinkConversation : Conversation
        {
            protected override void StartConversation(string converstationInitiator)
            {
                DialogueSystem.Instance.StartDialogue(converstationInitiator);
                DialogueSystem.Instance.WriteNPCLine("I'll have anything on the menu, as long as it's non-alcoholic.");
                DialogueSystem.Instance.WriteNPCLine(Random.value > 0.5 ? "...had a heavy one last night.": "Got a shift starting soon.");
                DialogueSystem.Instance.WritePlayerChoiceLine("<i>No problem.</i>", EndConversation(DialogueOutcome.Default));
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
                DialogueSystem.Instance.WritePlayerChoiceLine("<i>Sure.</i>", EndConversation(DialogueOutcome.Default));
            }
        }
    }
}
