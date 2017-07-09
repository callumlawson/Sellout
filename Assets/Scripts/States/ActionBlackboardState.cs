﻿using System;
using System.Text;
using Assets.Framework.Entities;
using Assets.Framework.States;

namespace Assets.Scripts.States
{
    [Serializable]
    class ActionBlackboardState : IState
    {
        public enum ReceiveItemDecisionResponse
        {
            None,
            GaveBack,
            ThrewOut,
            GaveOtherItem,
            Kept
        }

        public Entity TargetEntity;
        public bool Paused;
        public string CurrentActions;
        public ReceiveItemDecisionResponse ReceivedItemResponse;

        public ActionBlackboardState(Entity targetEntity)
        {
            TargetEntity = targetEntity;
        }

        public override string ToString()
        {
            var message = new StringBuilder();
            if (TargetEntity != null)
            {
                message.AppendLine("Target Entity: " + TargetEntity.EntityId);
            }
            else
            {
                message.AppendLine("Target Entity: None");
            }
            message.Append(CurrentActions);
            return message.ToString();
        }
    }
}
