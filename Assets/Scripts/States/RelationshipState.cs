using System;
using Assets.Framework.States;

namespace Assets.Scripts.States
{
    [Serializable]
    class RelationshipState : IState
    {
        //-ve doesn't like player. +ve likes player
        public int PlayerOpinion;

        public override string ToString()
        {
            return "PlayerOpinion: " + PlayerOpinion;
        }
    }
}
