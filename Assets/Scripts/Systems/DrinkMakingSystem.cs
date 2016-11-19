using Assets.Framework.States;
using Assets.Framework.Systems;
using Assets.Scripts.States;
using System.Collections.Generic;
using Assets.Scripts.Util;
using UnityEngine;
using Assets.Framework.Entities;

namespace Assets.Scripts.Systems
{
    class DrinkMakingSystem : IEntityManager, IInitSystem
    {
        private EntityStateSystem entitySystem;
        private DrinkUI ui;
        private Entity drinkTemplate;
        private DrinkState drinkState;

        public void SetEntitySystem(EntityStateSystem ess)
        {
            entitySystem = ess;
        }

        public void OnInit()
        {
            if (drinkTemplate == null)
            {
                drinkTemplate = entitySystem.CreateEntity(new List<IState>
                {
                    new DrinkState()
                });
            }

            drinkState = drinkTemplate.GetState<DrinkState>();
            drinkState.Clear();

            if (ui == null)
            {
                var uiResource = Resources.Load(Prefabs.DrinkMixingUI);
                var uiGameObject = UnityEngine.Object.Instantiate(uiResource) as GameObject;
                ui = uiGameObject.GetComponent<DrinkUI>();

                ui.onMixEvent += OnMixEvent;
                ui.onCloseEvent += OnCloseUI;
                ui.onIncrementIngredient += OnIncrementIngredient;
                ui.onDecrementIngredient += OnDecrementIngredient;
            }

            UpdateUI();

            ui.gameObject.SetActive(true);
        }

        private void OnDecrementIngredient(Ingredient ingredient)
        {
            drinkState.ChangeIngredientAmount(ingredient, -1);
            UpdateUI();
        }

        private void OnIncrementIngredient(Ingredient ingredient)
        {
            drinkState.ChangeIngredientAmount(ingredient, 1);
            UpdateUI();
        }

        public void OnCloseUI()
        {
            ui.gameObject.SetActive(false);
        }

        private void OnMixEvent()
        {
            MakeDrink(drinkState);

            drinkState.Clear();
            UpdateUI();
        }

        private void MakeDrink(DrinkState template)
        {
            var prefab = Prefabs.Drink;
            entitySystem.CreateEntity(new List<IState>
            {
                new PrefabState(prefab),
                new DrinkState(template)
            });
        }

        private void UpdateUI()
        {
            var contents = drinkState.GetContents();
            ui.UpdateUI(contents);
        }
    }
}
