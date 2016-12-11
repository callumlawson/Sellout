using System;
using Assets.Framework.States;

namespace Assets.Scripts.States
{
    [Serializable]
    class NameState : IState
    {
        public readonly string Name;

        public NameState(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
