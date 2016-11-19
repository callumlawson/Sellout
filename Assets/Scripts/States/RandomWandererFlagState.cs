using System;
using Assets.Framework.States;

namespace Assets.Scripts.States
{
    [Serializable]
    public class RandomWandererFlagState : IState
    {
        public override string ToString()
        {
            return "Is a Random Wanderer";
        }
    }
}
