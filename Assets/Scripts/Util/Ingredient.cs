using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Util
{
    public enum Ingredient
    {
        Synthol,
        Alcohol,
        Orange,
        Cola
    }

    public static class Ingredients
    {
        private static Color Orange = new Color(1.0f, 0.65f, 0.0f);

        public static Dictionary<Ingredient, Color> IngredientColorMap = new Dictionary<Ingredient, Color>
        {
            {Ingredient.Synthol, Color.white},
            {Ingredient.Alcohol, Color.green},
            {Ingredient.Orange, Orange},
            {Ingredient.Cola, Color.red}
        };
    }
}
