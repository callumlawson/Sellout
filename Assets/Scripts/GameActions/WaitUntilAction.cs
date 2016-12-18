﻿using Assets.Scripts.GameActions.Framework;
using System;
using Assets.Framework.Entities;
using Assets.Scripts.States;
using Assets.Framework.States;

namespace Assets.Scripts.GameActions
{
    class WaitUntilAction : GameAction
    {
        private TimeState timeState;
        private DateTime goalTime;

        public WaitUntilAction(DateTime goalTime)
        {
            this.goalTime = goalTime;
        }

        public override void OnFrame(Entity entity)
        {
            var currentTime = timeState.time;
            if (currentTime >= goalTime)
            {
                ActionStatus = ActionStatus.Succeeded;
            }
        }

        public override void OnStart(Entity entity)
        {
            timeState = StaticStates.Get<TimeState>();
        }

        public override void Pause()
        {
            
        }

        public override void Unpause()
        {
            
        }
    }
}