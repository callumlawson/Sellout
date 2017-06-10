using System.Collections.Generic;
using Assets.Framework.Entities;
using Assets.Scripts.GameActions;
using Assets.Scripts.Systems;
using Assets.Scripts.Util;
using Assets.Scripts.Visualizers;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Bar
{
    [UsedImplicitly]
    class MixologyBookUI : MonoBehaviour, IEntityVisualizer
    {
        #pragma warning disable 649
        [SerializeField] private GameObject recipeContentPane;
        [SerializeField] private GameObject recipeTemplate;
        [SerializeField] private GameObject ingredientTemplate;
        [SerializeField] private GameObject breakTemplate;
        [SerializeField] private Text nameText;
        [SerializeField] private Text orderText;
        [SerializeField] private Text speciesText;
        #pragma warning restore 649

        private const string RecipeToNamePath = "Title/Text";
        private const string IngredientToNamePath = "Panel/Name";
        private const string IngredientToAmounttPath = "Panel/Amount";

        private const string DefaultNameText = "????";
        private const string DefaultOrderText = "????";
        private const string DefaultSpeciesText = "????";

        private Dictionary<Ingredient, IngredientPanelUI> ingredientPanels;

        [UsedImplicitly]
        public void Awake()
        {
            for (var i = 0; i < DrinkRecipes.Recipes.Count; i++)
            {
                AddRecipe(DrinkRecipes.Recipes[i]);
                if (i < DrinkRecipes.Recipes.Count - 1)
                {
                    var breakUI = Instantiate(breakTemplate);
                    breakUI.transform.SetParent(recipeContentPane.transform);
                }
            }
        }

        public void OnStartRendering(Entity entity)
        {
            EventSystem.StartDrinkOrderEvent += OnStartDrinkOrder;
            EventSystem.EndDrinkOrderEvent += OnEndDrinkOrder;
        }

        private void OnStartDrinkOrder(DrinkOrders.DrinkOrder order)
        {
            if (order.OrderType == DrinkOrders.DrinkOrderType.Exact)
            {
                var exactOrder = (DrinkOrders.ExactDrinkorder) order;
                orderText.text = exactOrder.Recipe != null ? exactOrder.Recipe.DrinkName : DefaultOrderText;
            }
            nameText.text = order.OrdererName ?? DefaultNameText;
            speciesText.text = DrinkOrders.DrinkOrder.OrdererSpecies ?? DefaultSpeciesText;
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
                ingredientUI.transform.Find(IngredientToNamePath).GetComponent<Text>().text = ingredient.Key.ToString();
                ingredientUI.transform.Find(IngredientToAmounttPath).GetComponent<Text>().text = ingredient.Value.ToString();
            }
        }

        public void OnFrame()
        {
            // do nothing
        }

        public void OnStopRendering(Entity entity)
        {
            // do nothing
        }
    }
}
