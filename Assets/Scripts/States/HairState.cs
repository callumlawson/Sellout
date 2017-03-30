using Assets.Framework.States;
using Assets.Scripts.Util.NPCVisuals;
using System;
using Assets.Scripts.Util.NPC;

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
