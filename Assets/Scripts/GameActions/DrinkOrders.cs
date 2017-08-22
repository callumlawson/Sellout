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
using Assets.Scripts.Util.NPC;
using System.Collections.Generic;

namespace Assets.Scripts.GameActions
{
    public static class DrinkOrders
    {
        public enum DrinkOrderType
        {
            AkwaysSucceeds,
            Exact,
            NonAlcoholic,
            ContainingIngredient,
            ExcludingIngredient
        }

        public abstract class DrinkOrder
        {
            public const string OrdererSpecies = "Human";
            public string OrdererName;
            public readonly DrinkOrderType OrderType;
            public List<DrinkPredicate> DrinkPredicates;
            public string OrderedItem;

            protected DrinkOrder(DrinkOrderType orderType, string orderedItem)
            {
                OrderType = orderType;
                OrderedItem = orderedItem;
            }

            public bool IsValidForOrder(DrinkState drink, out IncorrectDrinkReason reason)
            {
                foreach (var predicate in DrinkPredicates)
                {
                    var success = predicate.test.Invoke(drink);
                    if (!success)
                    {
                        reason = predicate.failure;
                        return false;
                    }
                }

                reason = IncorrectDrinkReason.None;
                return true;
            }
        }

        public struct DrinkPredicate
        {
            public Func<DrinkState, bool> test;
            public IncorrectDrinkReason failure;

            public DrinkPredicate(Func<DrinkState, bool> test, IncorrectDrinkReason failure)
            {
                this.test = test;
                this.failure = failure;
            }
        }

        private static DrinkPredicate GlassHasContents = new DrinkPredicate(drink => drink.GetContents().Count != 0, IncorrectDrinkReason.EmptyGlass);
        private static DrinkPredicate RecipeExists = new DrinkPredicate(drink => DrinkRecipes.Contains(drink) || Equals(drink, DrinkRecipes.Beer.Contents), IncorrectDrinkReason.RecipeDoesNotExist);
        private static DrinkPredicate RecipeIsNonAlcoholic = new DrinkPredicate(drink => DrinkState.IsNonAlcoholic(drink), IncorrectDrinkReason.Alcoholic);

        public class AlwaysSucceedsDrinkOrder : DrinkOrder
        {
            public AlwaysSucceedsDrinkOrder() : base(DrinkOrderType.AkwaysSucceeds, "NONE")
            {
                DrinkPredicates = new List<DrinkPredicate>();
            }
        }

        public class ExactDrinkorder : DrinkOrder
        {
            public readonly DrinkRecipe Recipe;

            public ExactDrinkorder(DrinkRecipe recipe, string ordererName) : base(DrinkOrderType.Exact, recipe.DrinkName)
            {
                Recipe = recipe;
                OrdererName = ordererName;
                DrinkPredicates = new List<DrinkPredicate>()
                {
                    GlassHasContents,
                    new DrinkPredicate(testDrink => DrinkState.IsIdentical(recipe.Contents, testDrink), IncorrectDrinkReason.WrongRecipe)
                };  
            }

            public override string ToString()
            {
                return Recipe.DrinkName;
            }
        }

        public class NonAlcoholicDrinkOrder : DrinkOrder
        {
            public NonAlcoholicDrinkOrder(string ordererName) : base(DrinkOrderType.NonAlcoholic, "NONE")
            {
                OrdererName = ordererName;
                DrinkPredicates = new List<DrinkPredicate>()
                {
                    GlassHasContents,
                    RecipeIsNonAlcoholic,
                    RecipeExists
                };
            }

            public override string ToString()
            {
                return "Non Alcoholic";
            }
        }

        private class IncludingIngredientOrder : DrinkOrder
        {
            private readonly Ingredient ingredient;

            public IncludingIngredientOrder(Ingredient ingredient, string ordererName) : base(DrinkOrderType.ContainingIngredient, ingredient.ToString())
            {
                this.ingredient = ingredient;
                OrdererName = ordererName;
                DrinkPredicates = new List<DrinkPredicate>()
                {
                    GlassHasContents,
                    new DrinkPredicate(testDrink => testDrink.ContainsIngedient(this.ingredient), IncorrectDrinkReason.DoesNotContainIngredient),
                    RecipeExists
                };
            }

            public override string ToString()
            {
                return "With " + ingredient;
            }
        }

        private class ExcludingIngredientOrder : DrinkOrder
        {
            private readonly Ingredient ingredient;

            public ExcludingIngredientOrder(Ingredient ingredient, string ordererName) : base(DrinkOrderType.ExcludingIngredient, ingredient.ToString())
            {
                this.ingredient = ingredient;
                OrdererName = ordererName;

                DrinkPredicates = new List<DrinkPredicate>()
                {
                    GlassHasContents,
                    new DrinkPredicate(testDrink => !testDrink.ContainsIngedient(this.ingredient), IncorrectDrinkReason.ContainsIngredient),
                    RecipeExists
                };
            }

            public override string ToString()
            {
                return "Not " + ingredient;
            }
        }

        public static GameAction GetRandomOrder(Entity entity, int orderTimeOurInMins = 120)
        {
            var randomValue = Random.value;
            if (randomValue <= 0.20)
            {
                var drinkOrder = new ExactDrinkorder(DrinkRecipes.GetRandomDrinkRecipe(), entity.GetState<NameState>().Name);
                return OrderDrink(entity, drinkOrder, DialogueSelector.GetExactDrinkOrderConversation(drinkOrder.Recipe.DrinkName, entity), orderTimeoutInMins: orderTimeOurInMins);
            }
            if (randomValue <= 0.40)
            {
                return OrderDrink(entity, new NonAlcoholicDrinkOrder(entity.GetState<NameState>().Name), new OrderNonAlcoholicDrinkConversation(), orderTimeoutInMins: orderTimeOurInMins);
            }
            if (randomValue <= 0.60)
            {
                var ingredient = Ingredients.DispensedIngredients.PickRandom();
                return OrderDrink(entity, new IncludingIngredientOrder(ingredient, entity.GetState<NameState>().Name) , new OrderDrinkIncludingIngredientConversation(ingredient) , orderTimeoutInMins: orderTimeOurInMins);
            }
            if (randomValue <= 0.80)
            {
                var ingredient = Ingredients.DispensedIngredients.PickRandom();
                return OrderDrink(entity, new ExcludingIngredientOrder(ingredient, entity.GetState<NameState>().Name), new OrderDrinkExcludingIngredientConversation(ingredient), orderTimeoutInMins: orderTimeOurInMins);
            }
            return OrderDrink(entity, new ExactDrinkorder(DrinkRecipes.Beer, entity.GetState<NameState>().Name), DialogueSelector.GetExactDrinkOrderConversation("Beer", entity), orderTimeoutInMins: orderTimeOurInMins);
        }

        public static GameAction GetRandomAlcoholicDrinkOrder(Entity entity, Conversation correctDrinkConversation = null, Conversation incorrectDrinkConversation = null, Dictionary<String, GameAction> otherDrinkActions = null, int orderTimeOurInMins = 20)
        {
            var drinkOrder = new ExactDrinkorder(DrinkRecipes.GetRandomAlcoholicDrinkRecipe(), entity.GetState<NameState>().Name);
            return OrderDrink(entity, drinkOrder, DialogueSelector.GetExactDrinkOrderConversation(drinkOrder.Recipe.DrinkName, entity, required: Required.Yes), correctDrinkConversation, incorrectDrinkConversation, otherDrinkActions, orderTimeOurInMins);
        }

        public static GameAction GetRandomAlcoholicDrinkOrderWithoutFailure(Entity entity, int orderTimeOurInMins = 20)
        {
            var drinkOrder = new ExactDrinkorder(DrinkRecipes.GetRandomAlcoholicDrinkRecipe(), entity.GetState<NameState>().Name);
            return OrderDrinkWithoutFailure(entity, drinkOrder, DialogueSelector.GetExactDrinkOrderConversation(drinkOrder.Recipe.DrinkName, entity, required: Required.Yes), orderTimeOurInMins);
        }

        public static ActionSequence OrderDrink(Entity entity, DrinkOrder drinkOrder, Conversation conversation, Conversation correctDrinkConversation = null, Conversation incorrectDrinkConversation = null, Dictionary<String, GameAction> otherDrinkActions = null, int orderTimeoutInMins = 40)
        {
            var wrapper = new ActionSequence("DrinkOrderThenClear");
            var orderDrink = new ParallelUntilAllCompleteAction("OrderDrink");
            orderDrink.Add(new ReportSuccessDecorator(new ConversationAction(conversation)));
            var waitForDrink = new ConditionalActionSequence("WaitForDrink");
            waitForDrink.Add(new StartDrinkOrderAction(drinkOrder));
            waitForDrink.Add(CommonActions.WaitForDrink(entity, drinkOrder.OrderedItem, drinkOrder, orderTimeoutInMins, correctDrinkConversation: correctDrinkConversation, incorrectDrinkConversation: incorrectDrinkConversation, otherDrinkActions: otherDrinkActions));
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
            waitForDrink.Add(CommonActions.WaitForDrinkWithoutFailure(entity, drinkOrder, orderTimeoutInMins));
            orderDrink.Add(waitForDrink);
            return orderDrink;
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


        private class OrderDrinkExcludingIngredientConversation : Conversation
        {
            private readonly Ingredient ingredient;

            public OrderDrinkExcludingIngredientConversation(Ingredient ingredient)
            {
                this.ingredient = ingredient;
            }

            protected override void StartConversation(string converstationInitiator)
            {
                DialogueSystem.Instance.StartDialogue(converstationInitiator);
                DialogueSystem.Instance.WriteNPCLine("I'll take any drink without " + ingredient + ". Can't stand that stuff.");
                DialogueSystem.Instance.WriteNPCLine(Random.value > 0.5 ? "Otherwise I'm not fussy." : "I'm basically allergic.");
            }
        }
    }
}
