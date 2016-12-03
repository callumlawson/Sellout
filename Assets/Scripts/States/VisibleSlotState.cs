﻿using Assets.Framework.States;
using System;

namespace Assets.Scripts.States
{
    [Serializable]
    class VisibleSlotState : IState
    {
        public override string ToString()
        {
            return "Has visible slot";
        }
    }
}
