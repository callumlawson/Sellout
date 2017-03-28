using System;
using Assets.Framework.States;

namespace Assets.Scripts.States
{
    public enum EnvironmentQuality {
        Poor,
        Alright,
        Great
    }

    [Serializable]
    public class EnvironmentQualityState : IState
    {
        public EnvironmentQuality EnvironmentQuality;

        public EnvironmentQualityState()
        {
            EnvironmentQuality = EnvironmentQuality.Alright;
        }
    }
}
