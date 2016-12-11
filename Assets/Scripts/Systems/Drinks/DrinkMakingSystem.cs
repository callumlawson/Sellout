using Assets.Framework.States;
using Assets.Framework.Systems;
using Assets.Scripts.States;
using System.Collections.Generic;
using Assets.Scripts.Util;
using UnityEngine;
using Assets.Framework.Entities;
using Assets.Scripts.Util.Events;

namespace Assets.Scripts.Systems.Drinks
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
            
            if (ui == null)
            {
                var uiResource = Resources.Load(Prefabs.DrinkMixingUI);
                var uiGameObject = Object.Instantiate(uiResource) as GameObject;
                ui = uiGameObject.GetComponent<DrinkUI>();

                ui.onMixEvent += OnMixEvent;
                ui.onCloseEvent += OnCloseUI;
                ui.onIncrementIngredient += OnIncrementIngredient;
                ui.onDecrementIngredient += OnDecrementIngredient;
            }

            OnCloseUI();

            EventSystem.OpenDrinkMakingEvent += OnOpenUI;
            EventSystem.CloseDrinkMakingEvent += OnCloseUI;
        }

        private void OnOpenUI()
        {
            drinkState.Clear();
            UpdateUI();
            ui.gameObject.SetActive(true);
        }

        private void OnCloseUI()
        {
            ui.gameObject.SetActive(false);
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
        
        private void OnMixEvent()
        {
            var player = StaticStates.Get<PlayerState>().Player;
            if (player.GetState<InventoryState>().child == null)
            {
                var drink = MakeDrink(drinkState);
                EventSystem.BroadcastEvent(new InventoryRequestEvent(null, player, drink));

                drinkState.Clear();
                UpdateUI();
            }
            else
            {
                Debug.Log("Unable to make a drink because the player is already holding one!");
            }
        }

        private Entity MakeDrink(DrinkState template)
        {
            var prefab = Prefabs.Drink;
            return entitySystem.CreateEntity(new List<IState>
            {
                new PrefabState(prefab),
                new DrinkState(template),
                new PositionState(new Vector3(-9.68f, 1.27f, -14.27f))
            });
        }

        private void UpdateUI()
        {
            var contents = drinkState.GetContents();
            ui.UpdateUI(contents);
        }
    }
}
