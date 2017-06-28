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
using Assets.Scripts.States.Bar;
using Assets.Scripts.Visualizers.Bar;

namespace Assets.Scripts.Systems.Drinks
{
    class DrinkMakingSystem : IEntityManager, IInitSystem, IFrameSystem
    {
        private EntityStateSystem entitySystem;
        private DayPhaseState dayPhase;

        private float drinkDistanceFromCamera = 2.1f;

        private bool usingBar;

        private Entity mixologyBook;

        private PlayerState playerState;
        private Entity player;
        private InventoryState playerInventory;

        public void SetEntitySystem(EntityStateSystem ess)
        {
            entitySystem = ess;
        }

        public void OnInit()
        {
            EventSystem.StartDrinkMakingEvent += OnStartMakingDrink;
            EventSystem.onClickInteraction += OnClickInteraction;
            EventSystem.EndDrinkMakingEvent += StopMakingDrink;

            playerState = StaticStates.Get<PlayerState>();
            player = playerState.Player;
            playerInventory = player.GetState<InventoryState>();

            dayPhase = StaticStates.Get<DayPhaseState>();
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
                        case Prefabs.BeerStack:
                            PickUpStackItem(target);
                            break;
                        case Prefabs.Drink:
                            var drinkParent = target.GetState<InventoryState>().Parent;
                            if (drinkParent.HasState<ItemStackState>())
                            {
                                var itemStack = drinkParent;
                                PickUpStackItem(itemStack);
                            }
                            break;
                        case Prefabs.IngredientDispenser:
                            AddIngredientToDrink(target);
                            break;
                        case Prefabs.Washup:
                            if (playerInventory.Child != null && playerInventory.Child.GetState<PrefabState>().PrefabName != Prefabs.DispensingBottle)
                            {
                                WashUpItem(player);
                            }
                            break;
                        case Prefabs.Player:
                            if (playerInventory.Child != null)
                            {
                                GiveDrinkToPerson(target);
                            }
                            break;
                        case Prefabs.MixologyBook:
                            target.GetState<ActiveState>().IsActive = !target.GetState<ActiveState>().IsActive;
                            mixologyBook = target;
                            break;
                        case Prefabs.Person:
                            if (playerInventory.Child != null)
                            {
                                GiveDrinkToPerson(target);
                            }
                            break;
                        case Prefabs.ReceiveSpot:
                            TakeItemFromReceiveSpot(target);
                            break;
                        case Prefabs.Cubby:
                        case Prefabs.ServeSpot:
                            ExchangeItemWithSpot(target, targetPrefab.PrefabName);
                            break;
                        case Prefabs.DispensingBottle:
                            TakeItem(target);
                            break;
                        case Prefabs.DrinkSurface:
                            TryPutDownDispensingBottle();
                            break;
                        default:
                            if (target.HasState<InventoryState>())
                            {
                                var parent = target.GetState<InventoryState>().Parent;
                                var parentPrefab = parent.GetState<PrefabState>().PrefabName;
                                ExchangeItemWithSpot(target, parentPrefab);
                            }
                            break;
                    }
                } 
                else if (clickevent.MouseButton == 1 && dayPhase.CurrentDayPhase != DayPhase.Open && !playerState.CutsceneControlLock)
                {
                    EventSystem.EndDrinkMakingEvent.Invoke();
                }  
            }
        }

        private void TakeItemFromReceiveSpot(Entity target)
        {
            if (playerInventory.Child == null)
            {
                var itemInReceiveSpot = target.GetState<InventoryState>().Child;
                if (itemInReceiveSpot != null)
                {
                    EventSystem.ParentingRequestEvent.Invoke(new ParentingRequest { EntityFrom = target, EntityTo = player, Mover = itemInReceiveSpot });
                    InventoryItemColliderIsEnabled(false);
                }
            }
        }
        
        private void TakeItem(Entity targetItem)
        {
            if (playerInventory.Child == null && targetItem != null)
            {
                targetItem.GameObject.GetComponent<Collider>().enabled = false;
                EventSystem.ParentingRequestEvent.Invoke(new ParentingRequest { EntityFrom = null, EntityTo = player, Mover = targetItem });
            }
        }

        private void TryPutDownDispensingBottle()
        {
            if (playerInventory.Child != null && playerInventory.Child.GetState<PrefabState>().PrefabName == Prefabs.DispensingBottle)
            {
                InventoryItemColliderIsEnabled(true);
                EventSystem.ParentingRequestEvent.Invoke(new ParentingRequest { EntityFrom = player, EntityTo = null, Mover = playerInventory.Child});
            }
        }

        private void ExchangeItemWithSpot(Entity target, string spotPrefab)
        {
            if (spotPrefab != Prefabs.Cubby && spotPrefab != Prefabs.ReceiveSpot && spotPrefab != Prefabs.ServeSpot)
            {
                return;
            }
           
            var itemInReceiveSpot = target.GetState<InventoryState>().Child;

            if (playerInventory.Child == null && itemInReceiveSpot != null)
            {
                EventSystem.ParentingRequestEvent.Invoke(new ParentingRequest { EntityFrom = target, EntityTo = player, Mover = itemInReceiveSpot });
                InventoryItemColliderIsEnabled(false);
            }
            else if (playerInventory.Child != null && itemInReceiveSpot == null)
            {
                var playerItemPrefab = playerInventory.Child.GetState<PrefabState>().PrefabName;

                if (spotPrefab == Prefabs.Cubby && (playerItemPrefab == Prefabs.Beer || playerItemPrefab == Prefabs.Drink || playerItemPrefab == Prefabs.DispensingBottle))
                {
                    return;
                }

                if (spotPrefab == Prefabs.ServeSpot && playerItemPrefab != Prefabs.Drink)
                {
                    return;
                }

                InventoryItemColliderIsEnabled(true);
                EventSystem.ParentingRequestEvent.Invoke(new ParentingRequest { EntityFrom = player, EntityTo = target, Mover = playerInventory.Child });                
            }
        }
        
        private void PickUpStackItem(Entity stack)
        {
            if (playerInventory.Child != null)
            {
                var stackItemPrefabState = stack.GameObject.GetComponent<ItemStackVisualizer>().GetNewStackItem().Find(state => state.GetType() == typeof(PrefabState)) as PrefabState;
                if (playerInventory.Child.GetState<PrefabState>().PrefabName == stackItemPrefabState.PrefabName)
                {
                    var mover = playerInventory.Child;
                    EventSystem.ParentingRequestEvent.Invoke(new ParentingRequest { EntityFrom = player, EntityTo = null, Mover = mover });
                    entitySystem.RemoveEntity(mover);
                }

                return;
            }

            EventSystem.TakeStackItem(new TakeStackItemRequest { Requester = player, Stack = stack }); 
            InventoryItemColliderIsEnabled(false);
        }

        private void WashUpItem(Entity requester)
        {
            var item = requester.GetState<InventoryState>().Child;
            if (item != null)
            {
                EventSystem.ParentingRequestEvent.Invoke(new ParentingRequest { EntityFrom = requester, EntityTo = null, Mover = item });
                entitySystem.RemoveEntity(item);
            }
        }

        private void GiveDrinkToPerson(Entity person)
        {
            if (person.GetState<InventoryState>().Child == null)
            {
                EventSystem.ParentingRequestEvent.Invoke(new ParentingRequest
                {
                    EntityFrom = null,
                    EntityTo = person,
                    Mover = playerInventory.Child
                });
                InventoryItemColliderIsEnabled(true);
            }
        }

        public void OnFrame()
        {
            if (usingBar && playerInventory.Child != null)
            {
                var snapToSurface = playerInventory.Child.GetState<PrefabState>().PrefabName == Prefabs.DispensingBottle;
                var cursorState = StaticStates.Get<CursorState>();
                var selectedEntity = cursorState.SelectedEntity;
                if (selectedEntity == null)
                {
                    LerpDrinkPosition(GetNewHeldDrinkPosition(snapToSurface));
                    return;
                }
                var selectedPrefabType = selectedEntity.GetState<PrefabState>().PrefabName;
                if (playerInventory.Child != null &&
                    playerInventory.Child.GetState<PrefabState>().PrefabName != Prefabs.DispensingBottle
                    && (
                    selectedPrefabType == Prefabs.Counter ||
                    selectedPrefabType == Prefabs.ServeSpot ||
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
                        drinkPosition = GetNewHeldDrinkPosition(snapToSurface);
                    }
                    LerpDrinkPosition(drinkPosition);
                }
                else if (playerInventory.Child != null)
                {
                    LerpDrinkPosition(GetNewHeldDrinkPosition(snapToSurface));
                }
            }
            else if (!usingBar && playerInventory.Child != null)
            {
                playerInventory.Child.GameObject.transform.localPosition = Vector3.zero;
            }
        }

        private void AddIngredientToDrink(Entity dispenser)
        {
            if (playerInventory.Child != null && playerInventory.Child.GetState<DrinkState>().GetTotalDrinkSize() < Constants.MaxUnitsInDrink)
            {
                var ingredient = dispenser.GetState<DrinkState>().GetContents().Keys.First();
                dispenser.GameObject.GetComponent<OneShotAudioPlayer>().PlayOneShot();
                playerInventory.Child.GetState<DrinkState>().ChangeIngredientAmount(ingredient, 1);
            }
        }

        private void OnStartMakingDrink()
        {
            if (!usingBar)
            {
                playerState.PlayerStatus = PlayerStatus.Bar;
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

                if (playerInventory.Child != null)
                {
                    playerInventory.Child.GameObject.transform.localPosition = Vector3.zero;
                }

                usingBar = false;
                playerState.PlayerStatus = PlayerStatus.FreeMove;
            }
        }
        
        private void InventoryItemColliderIsEnabled(bool enable)
        {
            if (playerInventory.Child != null)
            {
                playerInventory.Child.GameObject.GetComponent<Collider>().enabled = enable;
            }
        }

        private void LerpDrinkPosition(Vector3 newDrinkPosition)
        {
            if (Vector3.Distance(playerInventory.Child.GameObject.transform.position, newDrinkPosition) > 5)
            {
                playerInventory.Child.GameObject.transform.position = newDrinkPosition;
            }
            playerInventory.Child.GameObject.transform.position = Vector3.Lerp(playerInventory.Child.GameObject.transform.position, newDrinkPosition, Time.deltaTime * 20);
        }

        private Vector3 GetNewHeldDrinkPosition(bool snapToSurface)
        {
            var mousePosition = Input.mousePosition;
            if (snapToSurface)
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 100))
                {
                    var objectHit = hit.collider.gameObject;
                    if (objectHit.CompareTag("DrinkSurface"))
                    {
                        return hit.point;
                    }
                }
            }
            return Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, drinkDistanceFromCamera));
        }
    }
}
