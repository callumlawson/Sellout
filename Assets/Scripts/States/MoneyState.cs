using System;
using Assets.Framework.States;

namespace Assets.Scripts.States
{
    [Serializable]
    class MoneyState : IState
    {
        public int CurrentMoney;

        public MoneyState(int money)
        {
            CurrentMoney = money;
        }

        public void ModifyMoney(int amount)
        {
            CurrentMoney += amount;
        }

        public override string ToString()
        {
            return string.Format("Money: {0}", CurrentMoney);
        }
    }
}
