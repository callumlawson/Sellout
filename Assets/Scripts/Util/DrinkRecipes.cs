using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.States;
using Random = System.Random;

namespace Assets.Scripts.Util
{
    public class DrinkRecipe
    {
        public string DrinkName { get; private set; }
        public DrinkState Contents { get; private set; }

        public DrinkRecipe(string drinkName, DrinkState contents)
        {
            DrinkName = drinkName;
            Contents = contents;
        }
    }

    public static class DrinkRecipes
    {
        private static readonly Random Random = new Random();

        public static readonly List<DrinkRecipe> Recipes = new List<DrinkRecipe>
        {
            new DrinkRecipe("Space Screwdriver", new DrinkState(new Dictionary<Ingredient, int> {{Ingredient.Alcohol, 1}, {Ingredient.OrangeJuice, 1}})),
            new DrinkRecipe("Space Rum and Cola", new DrinkState(new Dictionary<Ingredient, int> {{Ingredient.Alcohol, 1}, {Ingredient.Cola, 1}}))
        };

        public static DrinkRecipe GetRandomDrinkRecipe()
        {
            return Recipes.ElementAt(Random.Next(0, Recipes.Count));
        }
    }
}
