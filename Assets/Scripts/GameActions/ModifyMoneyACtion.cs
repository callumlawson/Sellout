using Assets.Framework.Entities;
using Assets.Framework.States;
using Assets.Scripts.GameActions.Framework;
using Assets.Scripts.States;
using Assets.Scripts.Util;
using Assets.Scripts.Visualizers;

namespace Assets.Scripts.GameActions
{
    class ModifyMoneyAction : GameAction
    {
        private readonly int moneyDelta;
        private readonly PaymentType paymentType;

        public ModifyMoneyAction(int moneyDelta, PaymentType paymentType)
        {
            this.moneyDelta = moneyDelta;
            this.paymentType = paymentType;
        }

        public override void OnStart(Entity entity)
        {
            if (moneyDelta > 0)
            {
                StandardSoundPlayer.Instance.PlaySfx(SFXEvent.Kaching);
            }
            StaticStates.Get<PaymentTrackerState>().AddPayment(moneyDelta, paymentType);
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
