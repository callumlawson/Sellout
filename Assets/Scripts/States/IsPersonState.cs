using System;
using Assets.Framework.States;

namespace Assets.Scripts.States
{
    [Serializable]
    public class IsPersonState : IState
    {
        public override string ToString()
        {
            return "Is Person";
        }
    }
}
