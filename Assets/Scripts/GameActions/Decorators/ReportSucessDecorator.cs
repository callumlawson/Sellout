using Assets.Framework.Entities;
using Assets.Scripts.GameActions.Framework;

namespace Assets.Scripts.GameActions.Decorators
{
    class ReportSuccessDecorator : GameAction
    {
        private readonly GameAction wrappedGameAction;

        public ReportSuccessDecorator(GameAction wrappedGameAction)
        {
            this.wrappedGameAction = wrappedGameAction;
        }

        public override void OnStart(Entity entity)
        {
            wrappedGameAction.OnStart(entity);
        }

        public override void OnFrame(Entity entity)
        {
            wrappedGameAction.OnFrame(entity);
            ActionStatus = ActionStatus.Succeeded;
        }

        public override void Pause()
        {
            wrappedGameAction.Pause();
        }

        public override void Unpause()
        {
            wrappedGameAction.Unpause();
        }

        public override string ToString()
        {
            return "[ReportSuccessDecorator] " + wrappedGameAction;
        }
    }
}
