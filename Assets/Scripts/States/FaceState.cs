using Assets.Framework.States;
using Assets.Scripts.Util.NPCVisuals;
using System;
using Assets.Scripts.Util.NPC;

namespace Assets.Scripts.States
{
    [Serializable]
    class FaceState : IState
    {
        public FaceType face;

        public FaceState(FaceType face)
        {
            this.face = face;
        }
    }
}
