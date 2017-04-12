﻿using System;
using Assets.Framework.States;

namespace Assets.Scripts.States
{
    [Serializable]
    public class PlayerDecisions : IState
    {
        public bool AcceptedDrugPushersOffer = true;
        public bool ToldInspectorAboutDrugPusher = false;
    }
}
