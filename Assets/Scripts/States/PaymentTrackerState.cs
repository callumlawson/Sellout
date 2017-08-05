using System;
using System.Collections.Generic;
using Assets.Framework.States;
namespace Assets.Scripts.States
{
    public enum PaymentType
    {
        DrinkSale,
        DrinkIngredient,
        DrugMoney
    }

    [Serializable]
    public class PaymentTrackerState : IState
    {
        public readonly Dictionary<PaymentType, int> TodaysPayments = new Dictionary<PaymentType, int>();

        public PaymentTrackerState()
        {
            TodaysPayments.Add(PaymentType.DrinkSale, 0);
            TodaysPayments.Add(PaymentType.DrinkIngredient, 0);
            TodaysPayments.Add(PaymentType.DrugMoney, 0);
        }

        public void AddPayment(int amount, PaymentType paymentType)
        {
            TodaysPayments[paymentType] += amount;
        }

        public void ClearOutcomes()
        {
            TodaysPayments.Clear();
        }
    }
}