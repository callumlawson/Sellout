using Assets.Framework.States;
using Assets.Scripts.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.States
{
    [Serializable]
    public class DrinkState : IState
    {
        [SerializeField] private readonly Dictionary<Ingredient, int> drinkContents;

        public Action DrinkAmountChanged = delegate {  };

        public DrinkState()
        {
            drinkContents = new Dictionary<Ingredient, int>();
        }

        public DrinkState(Dictionary<Ingredient, int> contents)
        {
            drinkContents = new Dictionary<Ingredient, int>(contents);
            DrinkAmountChanged.Invoke();
        }

        public DrinkState(DrinkState template)
        {
            drinkContents = new Dictionary<Ingredient, int>();
            foreach (var content in template.GetContents())
            {
                drinkContents.Add(content.Key, content.Value);
            }
        }
        
        public Dictionary<Ingredient, int> GetContents()
        {
            return drinkContents;
        }

        public void Clear()
        {
            drinkContents.Clear();
            DrinkAmountChanged.Invoke();
        }

        public void ChangeIngredientAmount(Ingredient ingredient, int delta)
        {
            var currentAmount = drinkContents.ContainsKey(ingredient) ? drinkContents[ingredient] : 0;
            var newAmount = Mathf.Clamp(currentAmount + delta, 0, int.MaxValue);            

            if (newAmount > 0)
            {
                drinkContents[ingredient] = currentAmount + delta;
            }
            else
            {
                drinkContents.Remove(ingredient);
            }
            DrinkAmountChanged.Invoke();
        }

        public int GetIngredientAmount(Ingredient ingredient)
        {
            return drinkContents[ingredient];
        }

        public int GetTotalDrinkSize()
        {
            var size = 0;
            foreach (var drinkToAmount in drinkContents)
            {
                size += drinkToAmount.Value;
            }
            return size;
        }

        public bool ContainsIngedient(Ingredient ingredient)
        {
            return drinkContents.ContainsKey(ingredient) && drinkContents[ingredient] >= 1;
        }

        public static bool IsNonAlcoholic(DrinkState drink)
        {
            return GetAlcoholAmount(drink) == 0;
        }

        public static bool IsIdentical(DrinkState goal, DrinkState drink)
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            return GetDifference(goal, drink) == 0.0f;
        }

        private static int GetAlcoholAmount(DrinkState drink)
        {
            var drinkContents = drink.GetContents();
            var alcholAmount = 0;
            foreach (var ingredientAmt in drinkContents)
            {
                if (Ingredients.IsAlcoholic(ingredientAmt.Key))
                {
                    alcholAmount += ingredientAmt.Value;
                }
            }
            return alcholAmount;
        }

        private static float GetDifference(DrinkState goal, DrinkState drink)
        {
            var goalContents = goal.GetContents();
            var drinkContents = drink.GetContents();

            var totalGoalIngredients = 0;
            var totalDifference = 0;

            foreach (var ingredientAmt in goalContents)
            {
                var ingredient = ingredientAmt.Key;
                var goalAmt = ingredientAmt.Value;

                totalGoalIngredients += goalAmt;

                int amtDrink;
                drinkContents.TryGetValue(ingredient, out amtDrink);

                var difference = Mathf.Abs(goalAmt - amtDrink);
                if (difference != 0)
                {
                    totalDifference += difference;
                }
            }

            foreach (var ingredientAmt in drinkContents)
            {
                var ingredient = ingredientAmt.Key;
                var amt = ingredientAmt.Value;

                if (!goalContents.ContainsKey(ingredient))
                {
                    totalDifference += amt;
                }
            }

            return Mathf.Min(totalDifference / (float)totalGoalIngredients, 1.0f);
        }

        public override string ToString()
        {
            var output = "Drink: \n";
            foreach (var content in drinkContents)
            {
                output = output + "\t(" + content.Key + "->" + content.Value + ")\n";
            }
            return output;
        }

        public override bool Equals(object otherDrinkState)
        {
            var item = otherDrinkState as DrinkState;

            if (item == null)
            {
                return false;
            }

            return drinkContents.Keys.All(ingredient => item.ContainsIngedient(ingredient)) &&
                   item.drinkContents.Keys.All(ContainsIngedient) &&
                   drinkContents.Keys.All(ingredient => item.GetIngredientAmount(ingredient) == GetIngredientAmount(ingredient));
        }

        public override int GetHashCode()
        {
            return drinkContents != null ? drinkContents.GetHashCode() : 0;
        }
    }
}
