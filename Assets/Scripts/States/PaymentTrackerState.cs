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
        private readonly Dictionary<PaymentType, int> todaysPayments = new Dictionary<PaymentType, int>();

        public void AddPayment(int amount, PaymentType paymentType)
        {
            if (todaysPayments.ContainsKey(paymentType))
            {
                todaysPayments[paymentType] += amount;
            }
            else
            {
                todaysPayments.Add(paymentType, amount);       
            }
        }

        public int GetPayment(PaymentType paymentType)
        {
            return todaysPayments.ContainsKey(paymentType) ? todaysPayments[paymentType] : 0;
        }

        public void ClearOutcomes()
        {
            todaysPayments.Clear();
        }
    }
}