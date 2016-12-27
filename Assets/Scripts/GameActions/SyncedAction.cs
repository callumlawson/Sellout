using Assets.Framework.Entities;
using Assets.Scripts.GameActions.Framework;

namespace Assets.Scripts.GameActions
{
    class SyncedAction : GameAction
    {
        private Entity one;
        private Entity two;

        private bool oneReady;
        private bool twoReady;

        public SyncedAction(Entity one, Entity two)
        {
            this.one = one;
            this.two = two;
        }

        public override void OnFrame(Entity entity)
        {
            
        }

        public override void OnStart(Entity entity)
        {
            if (entity == one)
            {
                oneReady = true;
            }

            if (entity == two)
            {
                twoReady = true;
            }

            if (oneReady && twoReady)
            {
                ActionStatus = ActionStatus.Succeeded;
            }
        }

        public override void Pause()
        {
            
        }

        public override void Unpause()
        {
            
        }
    }
}
