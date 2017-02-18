using Assets.Framework.Entities;
using Assets.Framework.States;
using Assets.Scripts.GameActions.Framework;
using Assets.Scripts.States;

namespace Assets.Scripts.GameActions
{
    class ModifyMoneyAction : GameAction
    {
        private readonly int moneyDelta;

        public ModifyMoneyAction(int moneyDelta)
        {
            this.moneyDelta = moneyDelta;
        }

        public override void OnStart(Entity entity)
        {
            StaticStates.Get<MoneyState>().ModifyMoney(moneyDelta);
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
