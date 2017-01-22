using Assets.Framework.States;
using Assets.Scripts.Util.NPCVisuals;
using System;

namespace Assets.Scripts.States
{
    [Serializable]
    class HairState : IState
    {
        public HairType hair;

        public HairState(HairType hair)
        {
            this.hair = hair;
        }
    }
}
