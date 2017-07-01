using Assets.Framework.Entities;
using Assets.Scripts.GameActions.Framework;
using Assets.Scripts.Systems.Bar;

namespace Assets.Scripts.GameActions.Inventory
{
    class QueueForBarWithPriority : GameAction
    {
        private GameAction actionsAtBar;

        public QueueForBarWithPriority(GameAction actionsAtBar)
        {
            this.actionsAtBar = actionsAtBar;
        }

        public override void OnStart(Entity entity)
        {
            BarQueueSystem.Instance.QueueEntityNext(entity, actionsAtBar);
            ActionStatus = ActionStatus.Succeeded;
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
