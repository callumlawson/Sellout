using Assets.Framework.Entities;
using Assets.Framework.States;
using Assets.Framework.Systems;
using Assets.Scripts.GameActions.Framework;
using Assets.Scripts.States;
using Assets.Scripts.States.Bar;
using Assets.Scripts.Systems;
using Assets.Scripts.Util;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GameActions.Inventory
{
    class PlaceItemInReceiveSpot : GameAction
    {
        public override void OnStart(Entity entity)
        {
            var receiveSpot = StaticStates.Get<BarEntities>().ReceiveSpot;            
            var inventoryItem = receiveSpot.GetState<InventoryState>().Child;

            if (inventoryItem == null)
            {
                bool success = CreateDrugsInReceiveSlot(receiveSpot);
                if (success)
                {
                    ActionStatus = ActionStatus.Succeeded;
                    return;
                }
            }

            ActionStatus = ActionStatus.Failed;
        }

        private bool CreateDrugsInReceiveSlot(Entity receiveSlot) {

            var newItem = EntityStateSystem.Instance.CreateEntity(new List<IState>
            {
                new PrefabState(Prefabs.Drugs),
                new DrinkState(new DrinkState()),
                new PositionState(receiveSlot.GameObject.transform.position),
                new InventoryState()
            });

            EventSystem.ParentingRequestEvent.Invoke(new ParentingRequest { EntityFrom = null, EntityTo = receiveSlot, Mover = newItem });

            if (receiveSlot.GetState<InventoryState>().Child != newItem)
            {
                Debug.LogError("Tried to add the newly created item to the stack but it failed!");
                EntityStateSystem.Instance.RemoveEntity(newItem);
                return false;
            }
            else
            {
                return true;
            }
        }

        public override void OnFrame(Entity entity)
        {
            //Nothing doing.
        }

        public override void Pause()
        {
            //Do Nothing
        }

        public override void Unpause()
        {
            //Do Nothing
        }
    }
}
