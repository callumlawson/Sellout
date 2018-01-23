using Assets.Scripts.GameActions;
using Assets.Scripts.Systems;
using Assets.Scripts.Util;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Bar
{
    [UsedImplicitly]
    class MixologyBookUI : MonoBehaviour
    {
        #pragma warning disable 649
        [SerializeField] private GameObject recipeContentPane;
        [SerializeField] private GameObject recipeTemplate;
        [SerializeField] private GameObject ingredientTemplate;
        [SerializeField] private GameObject breakTemplate;
        [SerializeField] public Button CloseButton;
        [SerializeField] private Text nameText;
        [SerializeField] private Text orderText;
        [SerializeField] private Text speciesText;
        #pragma warning restore 649

        private const string RecipeToNamePath = "Title/Text";
        private const string IngredientToNamePath = "Panel/Name";
        private const string IngredientToAmountPath = "Panel/Amount";

        [UsedImplicitly]
        public void Awake()
        {
            for (var i = 0; i < DrinkRecipes.Recipes.Count; i++)
            {
                AddRecipe(DrinkRecipes.Recipes[i]);
                if (i < DrinkRecipes.Recipes.Count - 1)
                {
                    //Remove breaks to try and avoid non scrolling issue.
                    //var breakUI = Instantiate(breakTemplate);
                    //breakUI.transform.SetParent(recipeContentPane.transform);
                }
            }

            EventSystem.StartDrinkOrderEvent += OnStartDrinkOrder;
            EventSystem.EndDrinkOrderEvent += OnEndDrinkOrder;
        }

        private void OnStartDrinkOrder(DrinkOrders.DrinkOrder order)
        {
            orderText.text = order.ToString();
            nameText.text = order.OrdererName;
            speciesText.text = DrinkOrders.DrinkOrder.OrdererSpecies;
        }

        private void OnEndDrinkOrder()
        {
            nameText.text = "";
            orderText.text = "";
            speciesText.text = "";
        }

        public void AddRecipe(DrinkRecipe recipe)
        {
            var recipeUI = Instantiate(recipeTemplate);
            recipeUI.transform.SetParent(recipeContentPane.transform);
            recipeUI.transform.Find(RecipeToNamePath).GetComponent<Text>().text = recipe.DrinkName;
            
            foreach (var ingredient in recipe.Contents.GetContents())
            {
                var ingredientUI = Instantiate(ingredientTemplate);
                ingredientUI.transform.SetParent(recipeUI.transform);
                ingredientUI.transform.Find(IngredientToNamePath).GetComponent<Text>().text = ingredient.Key != Ingredient.Beer ? ingredient.Key.ToString() : "Bottled";
                ingredientUI.transform.Find(IngredientToAmountPath).GetComponent<Text>().text = ingredient.Key != Ingredient.Beer ? ingredient.Value.ToString() : "";
            }
        }
    }
}
