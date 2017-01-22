using Assets.Framework.States;
using Assets.Framework.Systems;
using Assets.Scripts.States;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Util;
using UnityEngine;
using Assets.Framework.Entities;
using Assets.Scripts.Util.Events;
using DG.Tweening;

namespace Assets.Scripts.Systems.Drinks
{
    class NewDrinkMakingSystem : IEntityManager, IInitSystem, IFrameSystem
    {
        private EntityStateSystem entitySystem;

        private Entity camera;
        private Entity drink;

        private Vector3 initCameraPosition;
        private Vector3 initCameraRotation;

        private readonly Vector3 barCameraPosition = new Vector3(9.5f, 4f, 0);
        private readonly Vector3 barCameraRotation = new Vector3(20, -90, 0);

        private bool usingBar;
        private bool makingDrink;

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
                            PickUpGlass();
                            break;
                        case Prefabs.IngredientDispenser:
                            AddIngredientToDrink(target);
                            break;
                        case Prefabs.Washup:
                            WashUpGlass();
                            break;
                        case Prefabs.ServeSpot:
                            if (drink != null)
                            {
                                GiveDrinkToPlayer();
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
            if (drink != null)
            {
                makingDrink = false;
                entitySystem.RemoveEntity(drink);
                drink = null;
            }
        }

        private void GiveDrinkToPlayer()
        {
            var player = StaticStates.Get<PlayerState>().Player;
            if (player.GetState<HierarchyState>().Child == null)
            {
                EventSystem.ParentingRequestEvent.Invoke(new ParentingRequest
                {
                    EntityFrom = null,
                    EntityTo = player,
                    Mover = drink
                });
                DrinkColliderIsEnabled(true);
                drink = null;
                makingDrink = false;
            }
        }

        private void PickUpGlass()
        {
            if (!makingDrink)
            {
                makingDrink = true;
                drink = MakeDrink();
                DrinkColliderIsEnabled(false);
            }
        }

        public void OnFrame()
        {
            if (makingDrink)
            {
                var cursorState = StaticStates.Get<CursorState>();
                if (cursorState.SelectedEntity != null && drink != null && (
                    cursorState.SelectedEntity.GetState<PrefabState>().PrefabName == Prefabs.Counter ||
                    cursorState.SelectedEntity.GetState<PrefabState>().PrefabName == Prefabs.Washup))
                {
                    drink.GameObject.transform.position = cursorState.MousedOverPosition;
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
            EventSystem.EndDrinkMakingEvent.Invoke();
        }

        
        private Entity MakeDrink()
        {
            return MakeDrink(new DrinkState());
        }

        private Entity MakeDrink(DrinkState template)
        {
            return entitySystem.CreateEntity(new List<IState>
            {
                new PrefabState(Prefabs.Drink),
                new DrinkState(template),
                new PositionState(new Vector3(-9.68f, 1.27f, -14.27f)),
                new HierarchyState()
            });
        }

        private void DrinkColliderIsEnabled(bool enable)
        {
            drink.GameObject.GetComponent<Collider>().enabled = enable;
        }
    }
}
