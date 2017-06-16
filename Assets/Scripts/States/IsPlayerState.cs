using System;
using Assets.Framework.States;

namespace Assets.Scripts.States
{
    [Serializable]
    public class IsPlayerState : IState
    {
        public override string ToString()
        {
            return "Is Player";
        }
    }
}
