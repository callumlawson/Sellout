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
        public IngredientPanelUI ingredientTemplate;
        public RectTransform ingredientListParent;
#pragma warning restore 649

        public delegate void OnMixEvent();
        public delegate void OnCloseEvent();
        public delegate void OnIncrementIngredient(Ingredient ingredient);
        public delegate void OnDecrementIngredient(Ingredient ingredient);

        public OnMixEvent onMixEvent;
        public OnCloseEvent onCloseEvent;
        public OnIncrementIngredient onIncrementIngredient;
        public OnDecrementIngredient onDecrementIngredient;

        private Dictionary<Ingredient, IngredientPanelUI> ingredientPanels;

        public void Mix()
        {
            onMixEvent();
        }
        
        public void Close()
        {
            onCloseEvent();
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

        public void Awake()
        {
            ingredientPanels = new Dictionary<Ingredient, IngredientPanelUI>();
            foreach (Ingredient ingredient in Enum.GetValues(typeof(Ingredient)))
            {
                var panel = Instantiate(ingredientTemplate);
                panel.Initialize(ingredient, ingredientListParent);
                
                ingredientPanels.Add(ingredient, panel);
            }
        }
    }
}
