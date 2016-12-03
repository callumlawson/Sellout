using System;
using Assets.Framework.States;

namespace Assets.Scripts.States
{
    [Serializable]
    public class PersonState : IState
    {
        public override string ToString()
        {
            return "Is Person";
        }
    }
}
