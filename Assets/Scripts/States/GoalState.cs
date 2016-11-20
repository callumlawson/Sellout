using System;
using Assets.Framework.States;
using Assets.Scripts.Util;

namespace Assets.Scripts.States
{
    public enum GoalStatus
    {
        None,
        Start,
        Ongoing
    }

    [Serializable]
    class GoalState : IState
    {
        public Goal CurrentGoal { get; private set; }
        public Goal PreviousGoal { get; private set; }

        public GoalStatus CurrentGoalStatus { get; private set; }
        public GoalStatus PreviousGoalStatus { get; private set; }

        public void UpdateGoal(Goal goal)
        {
            if (goal != CurrentGoal)
            {
                PreviousGoal = CurrentGoal;
                CurrentGoal = goal;
            }
        }

        public void UpdateGoalStatus(GoalStatus status)
        {
            if (status != CurrentGoalStatus)
            {
                PreviousGoalStatus = CurrentGoalStatus;
                CurrentGoalStatus = status;
            }
        }

        public override string ToString()
        {
            return string.Format("Current Goal: {0} Previous Goal: {1} \n " +
                                 "Current Goal Status: {2} Previous Goal Status: {3}", 
                                 CurrentGoal, PreviousGoal, CurrentGoalStatus, PreviousGoalStatus);
        }
    }
}
