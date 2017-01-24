using Assets.Framework.States;
using Assets.Scripts.Util.NPCVisuals;
using System;

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
