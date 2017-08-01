using System;
using System.Collections.Generic;
using Assets.Framework.States;
namespace Assets.Scripts.States
{
    [Serializable]
    public class PaymentTrackerState : IState
    {
        public readonly List<string> TodaysPayments = new List<string>();

        public PaymentTrackerState()
        {
            TodaysPayments.Add("Test");
            TodaysPayments.Add("Test2");
        }

        public void AddOutcome(string outcome)
        {
            TodaysPayments.Add(outcome);
        }

        public void ClearOutcomes()
        {
            TodaysPayments.Clear();
        }
    }
}