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

namespace Assets.Scripts.Systems.Drinks
{
    class DrinkMakingSystem : IEntityManager, IInitSystem, IFrameSystem
    {
        private EntityStateSystem entitySystem;

        private Entity camera;
        private Entity drink;

        private Vector3 initCameraPosition;
        private Vector3 initCameraRotation;

        //TODO: Need a better way of doing this! Will break when we update the level.
        private readonly Vector3 barCameraPosition = new Vector3(9.5f, 4f, 0);
        private readonly Vector3 barCameraRotation = new Vector3(20, -90, 0);
        private readonly Vector3 drinkSpawnPoint = new Vector3(-0.652f, 1.178f, -2.368369f);

        private bool usingBar;
        private bool makingDrink;

        private Entity mixologyBook;

        public void SetEntitySystem(EntityStateSystem ess)
        {
            entitySystem = ess;
        }

        public void OnInit()
        {
            camera = StaticStates.Get<CameraState>().Camera;
            initCameraPosition = camera.GameObject.transform.position;
            initCameraRotation = camera.GameObject.transform.rotation.eulerAngles;

            EventSystem.StartDrinkMakingEvent += OnStartMakingDrink;
            EventSystem.onClickInteraction += OnClickInteraction;
        }

        private void OnClickInteraction(ClickEvent clickevent)
        {
            if (usingBar)
            {
                var target = clickevent.Target;
                if (target != null && target.HasState<PrefabState>())
                {
                    var targetPrefab = target.GetState<PrefabState>();

                    switch (targetPrefab.PrefabName)
                    {
                        case Prefabs.GlassStack:
                            if (!makingDrink)
                            {
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
                        default:
                            StopMakingDrink();
                            break;
                    }
                }
                else
                {
                    StopMakingDrink();
                }
            }
        }

        private void WashUpGlass()
        {
            makingDrink = false;
            entitySystem.RemoveEntity(drink);
            drink = null;
        }

        private void GiveDrinkToPerson(Entity person)
        {
            if (person.GetState<HierarchyState>().Child == null)
            {
                EventSystem.ParentingRequestEvent.Invoke(new ParentingRequest
                {
                    EntityFrom = null,
                    EntityTo = person,
                    Mover = drink
                });
                DrinkColliderIsEnabled(true);
                drink = null;
                makingDrink = false;
            }
        }

        private void PickUpGlass()
        {
            makingDrink = true;
            drink = MakeDrink();
            DrinkColliderIsEnabled(false);
        }   

        public void OnFrame()
        {
            if (makingDrink && usingBar)
            {
                var cursorState = StaticStates.Get<CursorState>();
                var selectedEntity = cursorState.SelectedEntity;
                if (selectedEntity == null)
                {
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
                    if (possibleSlot != null)
                    {
                        drinkPosition = possibleSlot.transform.position;
                    }
                    else
                    {
                        drinkPosition = cursorState.MousedOverPosition;
                    }
                    if (Vector3.Distance(drink.GameObject.transform.position, drinkPosition) > 5)
                    {
                        drink.GameObject.transform.position = drinkPosition;
                    }
                    drink.GameObject.transform.position = Vector3.Lerp(drink.GameObject.transform.position, drinkPosition, Time.deltaTime * 20);
                }
            }
        }

        private void AddIngredientToDrink(Entity dispenser)
        {
            if (drink != null)
            {
                var ingredient = dispenser.GetState<DrinkState>().GetContents().Keys.First();
                drink.GetState<DrinkState>().ChangeIngredientAmount(ingredient, 1);
            }
        }

        private void OnStartMakingDrink()
        {
            if (!usingBar)
            {
                usingBar = true;
                camera.GameObject.transform.DOMove(barCameraPosition, 1.0f);
                camera.GameObject.transform.DORotate(barCameraRotation, 1.0f);
            }
        }

        private void StopMakingDrink()
        {

            usingBar = false;
            camera.GameObject.transform.DOMove(initCameraPosition, 1.0f);
            camera.GameObject.transform.DORotate(initCameraRotation, 1.0f);
            if (mixologyBook != null)
            {
                mixologyBook.GetState<ActiveState>().IsActive = false;
            }
            EventSystem.EndDrinkMakingEvent.Invoke();
        }

        private Entity MakeDrink()
        {
            var entity = entitySystem.CreateEntity(new List<IState>
            {
                new PrefabState(Prefabs.Drink),
                new DrinkState(new DrinkState()),
                new PositionState(drinkSpawnPoint),
                new HierarchyState()
            });
            return entity;
        }

        private void DrinkColliderIsEnabled(bool enable)
        {
            drink.GameObject.GetComponent<Collider>().enabled = enable;
        }
    }
}
