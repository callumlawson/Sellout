using System;
using System.Collections.Generic;
using System.Text;
using Assets.Framework.States;
using Assets.Scripts.Util;

namespace Assets.Scripts.States
{
    [Serializable]
    class GoalSatisfierState : IState
    {
        public List<Goal> SatisfiedGoals;

        public GoalSatisfierState(List<Goal> satisfiedGoals)
        {
            SatisfiedGoals = satisfiedGoals;
        }

        public override string ToString()
        {
            var message = new StringBuilder();
            message.Append("GoalSatisfiers: ");
            foreach (var use in SatisfiedGoals)
            {
                message.Append(" " + use);
            }
            return message.ToString();
        }
    }
}
