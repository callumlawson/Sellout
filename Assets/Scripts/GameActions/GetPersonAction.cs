using Assets.Framework.Entities;
using Assets.Scripts.GameActions.Framework;
using Assets.Scripts.States;

namespace Assets.Scripts.GameActions
{
    class GetPersonAction : GameAction
    {
        private Entity goal;

        public GetPersonAction(Entity person)
        {
            goal = person;
        }

        public override void OnStart(Entity entity)
        {
            entity.GetState<ActionBlackboardState>().TargetWaypoint = goal;
            ActionStatus = ActionStatus.Succeeded;            
        }

        public override void OnFrame(Entity entity)
        {
            //Do Nothing
        }
    }
}
