using Assets.Framework.Entities;
using Assets.Scripts.GameActions.Framework;
using Assets.Scripts.States;

namespace Assets.Scripts.GameActions
{
    class SetTargetEntityAction : GameAction
    {
        private Entity targetEntity;

        public SetTargetEntityAction(Entity person)
        {
            targetEntity = person;
        }

        public override void OnStart(Entity entity)
        {
            entity.GetState<ActionBlackboardState>().TargetEntity = targetEntity;
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
