using Assets.Framework.States;
using Assets.Framework.Systems;
using Assets.Scripts.States;
using System.Collections.Generic;
using Assets.Scripts.Util;
using UnityEngine;

namespace Assets.Scripts.Systems
{
    class DrinkMakingSystem : IEntityManager, IInitSystem
    {
        private EntityStateSystem entitySystem;
        private DrinkUI ui;

        public void SetEntitySystem(EntityStateSystem ess)
        {
            entitySystem = ess;
        }

        public void OnInit()
        {
            if (ui == null)
            {
                var uiResource = Resources.Load(Prefabs.DrinkMixingUI);
                var uiGameObject = Object.Instantiate(uiResource) as GameObject;
                ui = uiGameObject.GetComponent<DrinkUI>();

                ui.onMixEvent += OnMixEvent;
                ui.onCloseEvent += CloseUI;
            }

            ui.gameObject.SetActive(true);
        }

        public void CloseUI()
        {
            ui.gameObject.SetActive(false);
        }

        private void OnMixEvent()
        {
            var drinkState = new DrinkState();
            drinkState.ChangeIngredientAmount(Ingredient.Alcohol, 1);
            MakeDrink(drinkState);
        }

        public void MakeDrink(DrinkState template)
        {
            var prefab = Prefabs.Drink;
            entitySystem.CreateEntity(new List<IState>
            {
                new PrefabState(prefab),
                new DrinkState(template),
                new PositionState(new Vector3(-9.68f, 1.27f, -14.27f))
            });
        }
    }
}
