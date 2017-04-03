using Assets.Framework.States;
using Assets.Framework.Systems;
using Assets.Scripts.States;
using System.Linq;
using Assets.Scripts.Util;
using UnityEngine;
using Assets.Framework.Entities;
using Assets.Scripts.Util.Events;
using Assets.Scripts.Visualizers;
using Assets.Scripts.Systems.Cameras;
using Assets.Scripts.Visualizers.Bar;
using Assets.Scripts.States.Bar;

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
        private Entity player;

        public void SetEntitySystem(EntityStateSystem ess)
        {
            entitySystem = ess;
        }

        public void OnInit()
        {
            EventSystem.StartDrinkMakingEvent += OnStartMakingDrink;
            EventSystem.onClickInteraction += OnClickInteraction;

            player = StaticStates.Get<PlayerState>().Player;
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
                                PickUpGlass(player, glassStack);
                            }
                            break;
                        case Prefabs.Drink:
                            if (drink == null)
                            {
                                var drinkParent = target.GetState<InventoryState>().Parent;
                                if (drinkParent.HasState<GlassStackState>())
                                {
                                    glassStack = drinkParent;
                                    drinkZValue = glassStack.GameObject.transform.position.z;
                                    PickUpGlass(player, glassStack);
                                }
                            }
                            break;
                        case Prefabs.IngredientDispenser:
                            AddIngredientToDrink(target);
                            break;
                        case Prefabs.Washup:
                            if (drink != null)
                            {
                                WashUpGlass(player);
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

        private void PickUpGlass(Entity requester, Entity stack)
        {
            if (!requester.HasState<InventoryState>())
            {
                Debug.LogError("Requester tried to pick up a glass but has no inventory state!");
            }

            if (requester.GetState<InventoryState>().Child != null)
            {
                return;
            }

            EventSystem.TakeGlass(new TakeGlassRequest { Requester = requester, stack = stack });
            
            if (requester.GetState<InventoryState>().Child == null || requester.GetState<InventoryState>().Child.GetState<PrefabState>().PrefabName != Prefabs.Drink)
            {
                Debug.LogErrorFormat("Tried to take glass, but came up with {0} instead!", requester.GetState<InventoryState>().Child);
            }

            drink = requester.GetState<InventoryState>().Child;
            DrinkColliderIsEnabled(false);
        }

        private void WashUpGlass(Entity requester)
        {
            EventSystem.ParentingRequestEvent.Invoke(new ParentingRequest { EntityFrom = requester, EntityTo = null, Mover = drink });
            entitySystem.RemoveEntity(drink);
            drink = null;
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
