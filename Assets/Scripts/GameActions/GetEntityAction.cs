using System;
using Assets.Framework.Entities;
using Assets.Scripts.GameActions.Framework;
using Assets.Scripts.States;

namespace Assets.Scripts.GameActions
{
    class GetEntityAction : GameAction
    {
        private Entity goal;

        public GetEntityAction(Entity person)
        {
            goal = person;
        }

        public override void OnStart(Entity entity)
        {
            entity.GetState<ActionBlackboardState>().TargetEntity = goal;
            ActionStatus = ActionStatus.Succeeded;            
        }

        public override void OnFrame(Entity entity)
        {
            //Do Nothing
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
