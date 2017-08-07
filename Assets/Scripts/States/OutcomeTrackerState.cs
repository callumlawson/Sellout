using System;
using System.Collections.Generic;
using Assets.Framework.States;
namespace Assets.Scripts.States
{
    [Serializable]
    public class OutcomeTrackerState : IState
    {
        public readonly List<string> TodaysOutcomes = new List<string>();

        public void AddOutcome(string outcome)
        {
            TodaysOutcomes.Add(outcome);
        }

        public void ClearOutcomes()
        {
            TodaysOutcomes.Clear();
        }
    }
}