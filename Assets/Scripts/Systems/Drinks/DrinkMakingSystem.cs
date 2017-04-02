using Assets.Framework.States;
using Assets.Framework.Systems;
using Assets.Scripts.States;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Util;
using UnityEngine;
using Assets.Framework.Entities;
using Assets.Scripts.Util.Events;
using Assets.Scripts.Visualizers;
using DG.Tweening;
using Assets.Scripts.Systems.Cameras;
using System;

namespace Assets.Scripts.Systems.Drinks
{
    class DrinkMakingSystem : IEntityManager, IInitSystem, IFrameSystem
    {
        private EntityStateSystem entitySystem;

        private Entity drink;

        private float drinkZValue = 0f;

        private bool usingBar;

        private Entity mixologyBook;
        private Entity glassStack;

        public void SetEntitySystem(EntityStateSystem ess)
        {
            entitySystem = ess;
        }

        public void OnInit()
        {
            EventSystem.StartDrinkMakingEvent += OnStartMakingDrink;
            EventSystem.onClickInteraction += OnClickInteraction;
        }

        private void OnClickInteraction(ClickEvent clickevent)
        {
            if (usingBar)
            {
                var target = clickevent.Target;
                if (clickevent.MouseButton == 0 && target != null && target.HasState<PrefabState>())
                {
                    var targetPrefab = target.GetState<PrefabState>();

                    switch (targetPrefab.PrefabName)
                    {
                        case Prefabs.GlassStack:
                            glassStack = target;
                            if (drink == null)
                            {
                                drinkZValue = glassStack.GameObject.transform.position.z;
                                PickUpGlass();
                            }
                            break;
                        case Prefabs.IngredientDispenser:
                            AddIngredientToDrink(target);
                            break;
                        case Prefabs.Washup:
                            if (drink != null)
                            {
                                WashUpGlass();
                            }
                            break;
                        case Prefabs.Player:
                            if (drink != null)
                            {
                                GiveDrinkToPerson(target);
                            }
                            break;
                        case Prefabs.MixologyBook:
                            target.GetState<ActiveState>().IsActive = !target.GetState<ActiveState>().IsActive;
                            mixologyBook = target;
                            break;
                        case Prefabs.Person:
                            if (drink != null)
                            {
                                GiveDrinkToPerson(target);
                            }
                            break;
                    }
                }
                else if (clickevent.MouseButton == 1)
                {
                    StopMakingDrink();
                }
            }
        }

        private void PickUpGlass()
        {
            drink = MakeDrink();
            DrinkColliderIsEnabled(false);
            glassStack.GameObject.SetActive(false);
        }

        private void WashUpGlass()
        {
            entitySystem.RemoveEntity(drink);
            drink = null;
            glassStack.GameObject.SetActive(true);
        }

        private void GiveDrinkToPerson(Entity person)
        {
            if (person.GetState<InventoryState>().Child == null)
            {
                EventSystem.ParentingRequestEvent.Invoke(new ParentingRequest
                {
                    EntityFrom = null,
                    EntityTo = person,
                    Mover = drink
                });
                DrinkColliderIsEnabled(true);
                drink = null;
                glassStack.GameObject.SetActive(true);
            }
        }

        public void OnFrame()
        {
            if (usingBar && drink != null)
            {
                var cursorState = StaticStates.Get<CursorState>();
                var selectedEntity = cursorState.SelectedEntity;
                if (selectedEntity == null)
                {
                    LerpDrinkPosition(GetNewHeldDrinkPosition());
                    return;
                }
                var selectedPrefabType = selectedEntity.GetState<PrefabState>().PrefabName;
                if (drink != null && (
                    selectedPrefabType == Prefabs.Counter ||
                    selectedPrefabType == Prefabs.Washup ||
                    selectedPrefabType == Prefabs.IngredientDispenser ||
                    selectedPrefabType == Prefabs.Person ||
                    selectedPrefabType == Prefabs.MixologyBook))
                {
                    var possibleSlot = selectedEntity.GameObject.GetComponentInChildren<SlotVisualizer>();
                    Vector3 drinkPosition;
                    if (possibleSlot != null && possibleSlot.CanHoldDrinks())
                    {
                        drinkPosition = possibleSlot.transform.position;
                    }
                    else
                    {
                        drinkPosition = GetNewHeldDrinkPosition();
                    }
                    LerpDrinkPosition(drinkPosition);
                }
                else if (drink != null)
                {
                    LerpDrinkPosition(GetNewHeldDrinkPosition());
                }
            }
            else if (!usingBar && drink != null)
            {
                LerpDrinkPosition(GetNewHeldDrinkPosition());
            }
        }

        private void AddIngredientToDrink(Entity dispenser)
        {
            if (drink != null && drink.GetState<DrinkState>().GetTotalDrinkSize() < Constants.MaxUnitsInDrink)
            {
                var ingredient = dispenser.GetState<DrinkState>().GetContents().Keys.First();
                drink.GetState<DrinkState>().ChangeIngredientAmount(ingredient, 1);
            }
        }

        private void OnStartMakingDrink()
        {
            if (!usingBar)
            {
                CameraSystem.GetCameraSystem().SetCameraMode(CameraSystem.CameraMode.Bar);
                usingBar = true;
            }
        }

        private void StopMakingDrink()
        {
            if (usingBar)
            {
                CameraSystem.GetCameraSystem().SetCameraMode(CameraSystem.CameraMode.Following);
                if (mixologyBook != null)
                {
                    mixologyBook.GetState<ActiveState>().IsActive = false;
                }
                EventSystem.EndDrinkMakingEvent.Invoke();
                usingBar = false;
            }
        }

        private Entity MakeDrink()
        {
            var entity = entitySystem.CreateEntity(new List<IState>
            {
                new PrefabState(Prefabs.Drink),
                new DrinkState(new DrinkState()),
                new PositionState(GetNewHeldDrinkPosition()),
                new InventoryState()
            });
            return entity;
        }

        private void DrinkColliderIsEnabled(bool enable)
        {
            drink.GameObject.GetComponent<Collider>().enabled = enable;
        }

        private void LerpDrinkPosition(Vector3 newDrinkPosition)
        {
            if (Vector3.Distance(drink.GameObject.transform.position, newDrinkPosition) > 5)
            {
                drink.GameObject.transform.position = newDrinkPosition;
            }
            drink.GameObject.transform.position = Vector3.Lerp(drink.GameObject.transform.position, newDrinkPosition, Time.deltaTime * 20);
        }

        private Vector3 GetNewHeldDrinkPosition()
        {
            var mousePosition = Input.mousePosition;
            var camera = Camera.main;
            var distanceFromCamera = drinkZValue - camera.transform.position.z;
            var worldPoint = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, distanceFromCamera));
            return worldPoint;
        }
    }
}
