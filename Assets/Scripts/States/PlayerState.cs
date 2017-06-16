using System;
using Assets.Framework.Entities;
using Assets.Framework.States;
namespace Assets.Scripts.States
{
    [Serializable]
    class PlayerState : IState
    {
        public readonly Entity Player;
        public bool CutsceneControlLock;
        public bool IsUsingBar;

        public PlayerState(Entity player, bool cutsceneControlLock, bool isUsingBar)
        {
            Player = player;
            CutsceneControlLock = cutsceneControlLock;
            IsUsingBar = isUsingBar;
        }

        public override string ToString()
        {
            return "Player: " + Player.EntityId;
        }
    }
}
