using Assets.Scripts.Util;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    [UsedImplicitly]
    class MixologyBookUI : MonoBehaviour
    {
        #pragma warning disable 649
        [SerializeField] private GameObject recipeContentPane;
        [SerializeField] private GameObject recipeTemplate;
        [SerializeField] private GameObject ingredientTemplate;
        #pragma warning restore 649

        private string recipeToNamePath = "Title/Text";
        private string ingredientToNamePath = "Panel/Name";
        private string ingredientToAmounttPath = "Panel/Amount";

        public void Close()
        {
            gameObject.SetActive(false);
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
    }
}
