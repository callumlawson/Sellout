using Assets.Scripts.Systems;
using Assets.Scripts.UI.Bar;
using Assets.Scripts.Util;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    class IngredientPanelUI : MonoBehaviour
    {
#pragma warning disable 649
        public Text ingredientName;
        public Text ingredientAmount;
#pragma warning restore 649

        private DrinkUI drinkUI;
        private Ingredient ingredient;

        public void Initialize(Ingredient ingredient, RectTransform parent)
        {
            this.ingredient = ingredient;
            ingredientName.text = ingredient.ToString();
            ingredientAmount.text = 0.ToString();

            transform.SetParent(parent);

            drinkUI = GetComponentInParent<DrinkUI>();
        }

        public void SetAmount(int amount)
        {
            ingredientAmount.text = amount.ToString();
        }

        public void IncrementIngredient()
        {
            drinkUI.IncrementIngredient(ingredient);
        }

        public void DecrementIngredient()
        {
            drinkUI.DecrementIngredient(ingredient);
        }
    }
}
