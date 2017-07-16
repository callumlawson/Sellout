using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Util.NPC
{

    public enum JobType
    {
        None,
        Crew,
        Trader,
        Diplomat,
        Traveller
    }

    public class JobTypeUtil
    {
        private static List<JobType> NonCrewJobTypes = new List<JobType>() {
            JobType.Trader,
            JobType.Diplomat,
            JobType.Traveller
        };

        public static JobType GetRandomNonCrewJobType()
        {
            var choice = Random.Range(0, NonCrewJobTypes.Count);
            return NonCrewJobTypes[choice];
        }
    }
}
