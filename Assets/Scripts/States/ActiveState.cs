using System;
using Assets.Framework.States;

namespace Assets.Scripts.States
{
    [Serializable]
    class ActiveState : IState
    {
        public bool IsActive;

        public ActiveState(bool isActive)
        {
            IsActive = isActive;
        }

        public override string ToString()
        {
            return IsActive ? "Is Active" : "Is NOT Active";
        }
    }
}
