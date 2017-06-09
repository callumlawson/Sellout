﻿using System.Collections.Generic;
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
        public static List<Ingredient> AlcoholicIngredients = new List<Ingredient> { Ingredient.Synthol, Ingredient.Alcohol };

        private static readonly Color Orange = new Color(1.0f, 0.65f, 0.0f);

        public static readonly Dictionary<Ingredient, Color> IngredientColorMap = new Dictionary<Ingredient, Color>
        {
            {Ingredient.Synthol, Color.white},
            {Ingredient.Alcohol, Color.green},
            {Ingredient.Orange, Orange},
            {Ingredient.Cola, Color.red}
        };

        public static bool IsAlcoholic(Ingredient ingredient)
        {
            return AlcoholicIngredients.Contains(ingredient);
        }
    }
}
