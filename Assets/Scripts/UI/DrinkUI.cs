using Assets.Scripts.States;
using Assets.Scripts.UI;
using Assets.Scripts.Util;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Systems
{
    class DrinkUI : MonoBehaviour
    {
#pragma warning disable 649
        [SerializeField] private MixologyBookUI mixologyBook;
        public IngredientPanelUI ingredientTemplate;
        public RectTransform ingredientListParent;
#pragma warning restore 649

        public delegate void OnMixEvent();
        public delegate void OnCloseEvent();
        public delegate void OnIncrementIngredient(Ingredient ingredient);
        public delegate void OnDecrementIngredient(Ingredient ingredient);

        public OnMixEvent onMixEvent = null;
        public OnCloseEvent onCloseEvent = null;
        public OnIncrementIngredient onIncrementIngredient = null;
        public OnDecrementIngredient onDecrementIngredient = null;

        private Dictionary<Ingredient, IngredientPanelUI> ingredientPanels;

        public static readonly Dictionary<Ingredient, int> screwdriverIngredients = new Dictionary<Ingredient, int>
        {
            {Ingredient.Alcohol, 1},
            {Ingredient.OrangeJuice, 1}
        };

        private static readonly Dictionary<Ingredient, int> rumAndColaIngredients = new Dictionary<Ingredient, int>
        {
            {Ingredient.Alcohol, 1},
            {Ingredient.Cola, 1}
        };

        private List<DrinkRecipe> recipes = new List<DrinkRecipe>
        {
            new DrinkRecipe("Space Screwdriver", new DrinkState(screwdriverIngredients)),
            new DrinkRecipe("Space Rum and Cola", new DrinkState(rumAndColaIngredients))
        };

        public void Awake()
        {
            ingredientPanels = new Dictionary<Ingredient, IngredientPanelUI>();
            foreach (Ingredient ingredient in Enum.GetValues(typeof(Ingredient)))
            {
                var panel = Instantiate(ingredientTemplate);
                panel.Initialize(ingredient, ingredientListParent);

                ingredientPanels.Add(ingredient, panel);
            }

            for (var i = 0; i < recipes.Count; i++)
            {
                mixologyBook.AddRecipe(recipes[i]);
            }
        }

        public void Mix()
        {
            onMixEvent();
        }
        
        public void Close()
        {
            mixologyBook.gameObject.SetActive(false);
            onCloseEvent();
        }

        public void OpenMixologyBook()
        {
            mixologyBook.gameObject.SetActive(true);
        }

        public void IncrementIngredient(Ingredient ingredient)
        {
            if (onIncrementIngredient != null)
            {
                onIncrementIngredient(ingredient);
            }
        }

        public void DecrementIngredient(Ingredient ingredient)
        {
            if (onDecrementIngredient != null)
            {
                onDecrementIngredient(ingredient);
            }
        }

        internal void UpdateUI(Dictionary<Ingredient, int> contents)
        {
            foreach (var panel in ingredientPanels)
            {
                var ingredient = panel.Key;
                var amount = contents.ContainsKey(ingredient) ? contents[ingredient] : 0;
                ingredientPanels[ingredient].SetAmount(amount);
            }
        }
    }
}
