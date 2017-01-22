using System;
using Assets.Framework.States;

namespace Assets.Scripts.States
{
    [Serializable]
    class NameState : IState
    {
        public readonly string Name;
        public float VerticalOffset;

        public NameState(string name, float verticalOffset)
        {
            Name = name;
            VerticalOffset = verticalOffset;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
