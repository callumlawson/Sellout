using System;
using Assets.Framework.States;
using Assets.Scripts.Util.NPC;

namespace Assets.Scripts.States.NPC
{
    [Serializable]
    class JobState : IState
    {
        public JobType job;

        public JobState(JobType job)
        {
            this.job = job;
        }

        public override string ToString()
        {
            return "Job: " + job.ToString();
        }
    }
}
