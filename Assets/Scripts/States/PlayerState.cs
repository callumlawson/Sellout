using System;
using Assets.Framework.Entities;
using Assets.Framework.States;
namespace Assets.Scripts.States
{
    public enum PlayerStatus
    {
        FreeMove,
        Cutscene,
        Bar
    }

    [Serializable]
    class PlayerState : IState
    {
        public readonly Entity Player;
        public bool CutsceneControlLock;
        public PlayerStatus PlayerStatus;

        public PlayerState(Entity player, bool cutsceneControlLock)
        {
            PlayerStatus = PlayerStatus.FreeMove;
            Player = player;
            CutsceneControlLock = cutsceneControlLock;
        }

        public override string ToString()
        {
            return "Player: " + Player.EntityId;
        }
    }
}
