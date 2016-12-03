using Assets.Scripts.States;
using UnityEngine;

namespace Assets.Scripts.Util
{
    class DrinkUtil
    {
        public static bool IsAlcoholic(Ingredient ingredient)
        {
            return ingredient == Ingredient.Synthol || ingredient == Ingredient.Alcohol;
        }

        public static int GetAlcoholAmount(DrinkState drink)
        {
            var drinkContents = drink.GetContents();
            var alcholAmount = 0;
            foreach (var ingredientAmt in drinkContents)
            {
                if (IsAlcoholic(ingredientAmt.Key))
                {
                    alcholAmount += ingredientAmt.Value;
                }
            }
            return alcholAmount;
        }

        public static float GetSimilarity(DrinkState goal, DrinkState drink)
        {
            var goalContents = goal.GetContents();
            var drinkContents = drink.GetContents();

            int totalGoalIngredients = 0;

            int totalDifference = 0;
            foreach (var ingredientAmt in goalContents)
            {
                var ingredient = ingredientAmt.Key;
                var goalAmt = ingredientAmt.Value;

                totalGoalIngredients += goalAmt;

                var amtDrink = 0;
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
    }
}
