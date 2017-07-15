using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Util
{
    public enum Ingredient
    {
        Synthol,
        Rum,
        Vodka,
        Orangeade,
        Cola,
        Beer
    }

    public static class Ingredients
    {
        public static readonly List<Ingredient> AlcoholicIngredients = new List<Ingredient> { Ingredient.Synthol, Ingredient.Rum, Ingredient.Vodka, Ingredient.Beer };
        public static readonly List<Ingredient> DispensedIngredients = new List<Ingredient> { Ingredient.Orangeade, Ingredient.Cola, Ingredient.Synthol, Ingredient.Rum, Ingredient.Vodka };

        private static readonly Color Orange = new Color(1.0f, 0.65f, 0.0f);
        private static readonly Color Brown = new Color(69.0f/255.0f, 56.0f/255.0f, 35.0f/255.0f);

        public static readonly Dictionary<Ingredient, Color> IngredientColorMap = new Dictionary<Ingredient, Color>
        {
            {Ingredient.Synthol, Color.green},
            {Ingredient.Rum, Color.red},
            {Ingredient.Vodka, Color.white },
            {Ingredient.Orangeade, Orange},
            {Ingredient.Cola, Color.black},
            {Ingredient.Beer, Brown }
        };

        public static bool IsAlcoholic(Ingredient ingredient)
        {
            return AlcoholicIngredients.Contains(ingredient);
        }
    }
}
