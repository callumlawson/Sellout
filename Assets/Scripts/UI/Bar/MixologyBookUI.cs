using System;
using System.Collections.Generic;
using Assets.Scripts.Util;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Systems;
using Assets.Scripts.Visualizers;
using Assets.Framework.Entities;

namespace Assets.Scripts.UI
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

        private string recipeToNamePath = "Title/Text";
        private string ingredientToNamePath = "Panel/Name";
        private string ingredientToAmounttPath = "Panel/Amount";

        private readonly string DefaultNameText = "????";
        private readonly string DefaultOrderText = "????";
        private readonly string DefaultSpeciesText = "????";

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
            Debug.Log("Start rendering");
            EventSystem.StartDrinkOrderEvent += OnStartDrinkOrder;
            EventSystem.EndDrinkOrderEvent += OnEndDrinkOrder;
        }

        public void OnStartDrinkOrder(DrinkOrder order)
        {
            Debug.Log("Drink order started.");

            nameText.text = order.OrdererName != null ? order.OrdererName : DefaultNameText;
            orderText.text = order.Recipe != null ? order.Recipe.DrinkName : DefaultOrderText;
            speciesText.text = order.OrdererSpecies != null ? order.OrdererSpecies : DefaultSpeciesText;
        }

        public void OnEndDrinkOrder()
        {
            nameText.text = "";
            orderText.text = "";
            speciesText.text = "";
        }

        public void AddRecipe(DrinkRecipe recipe)
        {
            var recipeUI = Instantiate(recipeTemplate);
            recipeUI.transform.SetParent(recipeContentPane.transform);
            recipeUI.transform.Find(recipeToNamePath).GetComponent<Text>().text = recipe.DrinkName;
            
            foreach (var ingredient in recipe.Contents.GetContents())
            {
                var ingredientUI = Instantiate(ingredientTemplate);
                ingredientUI.transform.SetParent(recipeUI.transform);
                ingredientUI.transform.Find(ingredientToNamePath).GetComponent<Text>().text = ingredient.Key.ToString();
                ingredientUI.transform.Find(ingredientToAmounttPath).GetComponent<Text>().text = ingredient.Value.ToString();
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
