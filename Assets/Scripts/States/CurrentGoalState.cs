using System;
using Assets.Framework.States;
using Assets.Scripts.Util;

namespace Assets.Scripts.States
{
    [Serializable]
    class CurrentGoalState : IState
    {
        public Goal CurrentGoal;

        public CurrentGoalState(Goal currentGoal)
        {
            CurrentGoal = currentGoal;
        }

        public override string ToString()
        {
            return string.Format("Current Goal: {0}", CurrentGoal);
        }
    }
}
