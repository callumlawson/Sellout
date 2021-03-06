﻿using Assets.Framework.Entities;
using Assets.Framework.States;
using Assets.Framework.Util;
using Assets.Scripts.GameActions.Framework;
using Assets.Scripts.States;
using Assets.Scripts.States.Bar;
using Assets.Scripts.Util;
using UnityEngine;

namespace Assets.Scripts.GameActions.Bar
{
    public class WaitForReceivedItemDecision : GameAction
    {
        private Entity item;

        public override void OnStart(Entity entity)
        {
            //TODO(Caryn): Race condition!!!!!!

            var receiveSpot = StaticStates.Get<BarEntities>().ReceiveSpot;

            item = receiveSpot.GetState<InventoryState>().Child;

            if (item == null)
            {
                Debug.LogError("Tried to put item into receive slot and failed!!!");
                ActionStatus = ActionStatus.Failed;
            }
        }

        public override void OnFrame(Entity entity)
        {
            if (item == null || item.GameObject.activeInHierarchy == false || item.GameObject.GetComponent<EntityIdComponent>().EntityId == -1)
            {
                item = null;
                entity.GetState<ActionBlackboardState>().ReceivedItemResponse = ActionBlackboardState.ReceiveItemDecisionResponse.ThrewOut;
                ActionStatus = ActionStatus.Succeeded;
                return;
            }

            var itemParent = item.GetState<InventoryState>().Parent;

            if (itemParent == null)
            {
                return;
            }

            if (itemParent == entity)
            {
                entity.GetState<ActionBlackboardState>().ReceivedItemResponse = ActionBlackboardState.ReceiveItemDecisionResponse.GaveBack;
                ActionStatus = ActionStatus.Succeeded;
                return;
            }

            if (itemParent.GetState<PrefabState>().PrefabName == Prefabs.Cubby)
            {
                entity.GetState<ActionBlackboardState>().ReceivedItemResponse = ActionBlackboardState.ReceiveItemDecisionResponse.Kept;
                ActionStatus = ActionStatus.Succeeded;
                return;
            }

            if (entity.GetState<InventoryState>().Child != null)
            {
                entity.GetState<ActionBlackboardState>().ReceivedItemResponse = ActionBlackboardState.ReceiveItemDecisionResponse.GaveOtherItem;
                ActionStatus = ActionStatus.Succeeded;
                return;
            }
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
