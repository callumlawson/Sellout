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

        public static readonly DrinkRecipe Beer = new DrinkRecipe("Beer", new DrinkState(new Dictionary<Ingredient, int> { { Ingredient.Beer, 5 } }));

        public static readonly List<DrinkRecipe> Recipes = new List<DrinkRecipe>
        {
            new DrinkRecipe("Space Screwdriver", new DrinkState(new Dictionary<Ingredient, int> {{Ingredient.Alcohol, 1}, {Ingredient.Orangeade, 1}})),
            new DrinkRecipe("Space Rum and Cola", new DrinkState(new Dictionary<Ingredient, int> {{Ingredient.Alcohol, 1}, {Ingredient.Cola, 1}})),
            new DrinkRecipe("Mind Meld", new DrinkState(new Dictionary<Ingredient, int> {{Ingredient.Synthol, 3}, {Ingredient.Alcohol, 1}})),
            new DrinkRecipe("Frosted Mind Meld", new DrinkState(new Dictionary<Ingredient, int> {{Ingredient.Synthol, 3}, {Ingredient.Alcohol, 1}, { Ingredient.Cola, 1 }})),
            new DrinkRecipe("Orangeola", new DrinkState(new Dictionary<Ingredient, int> {{Ingredient.Cola, 1}, {Ingredient.Orangeade, 1}})),
            new DrinkRecipe("Long Cola", new DrinkState(new Dictionary<Ingredient, int> {{Ingredient.Cola, 3}})),
            Beer
        };

        public static bool Contains(DrinkState drinkState)
        {
            return Recipes.Any(recipe => recipe.Contents.Equals(drinkState));
        }

        public static DrinkRecipe GetRandomDrinkRecipe()
        {
            return Recipes.ElementAt(Random.Next(0, Recipes.Count));
        }

        public static DrinkRecipe GetDrinkRecipe(string drinkName)
        {
            return Recipes.FirstOrDefault(recipe => recipe.DrinkName == drinkName);
        }
    }
}
