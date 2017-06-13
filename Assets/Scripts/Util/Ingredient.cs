using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Util
{
    public enum Ingredient
    {
        Synthol,
        Alcohol,
        Orangeade,
        Cola,
        Beer
    }

    public static class Ingredients
    {
        public static readonly List<Ingredient> AlcoholicIngredients = new List<Ingredient> { Ingredient.Synthol, Ingredient.Alcohol, Ingredient.Beer };
        public static readonly List<Ingredient> DispensedNonAlcoholicIngredients = new List<Ingredient> { Ingredient.Orangeade, Ingredient.Cola };

        private static readonly Color Orange = new Color(1.0f, 0.65f, 0.0f);
        private static readonly Color Brown = new Color(69.0f/255.0f, 56.0f/255.0f, 35.0f/255.0f);

        public static readonly Dictionary<Ingredient, Color> IngredientColorMap = new Dictionary<Ingredient, Color>
        {
            {Ingredient.Synthol, Color.green},
            {Ingredient.Alcohol, Color.white},
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
