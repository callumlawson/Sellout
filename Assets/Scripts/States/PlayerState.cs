using System;
using Assets.Framework.Entities;
using Assets.Framework.States;
namespace Assets.Scripts.States
{
    [Serializable]
    class PlayerState : IState
    {
        public readonly Entity Player;
        public bool TutorialControlLock;
        public bool IsUsingBar;

        public PlayerState(Entity player, bool tutorialControlLock, bool isUsingBar)
        {
            Player = player;
            TutorialControlLock = tutorialControlLock;
            IsUsingBar = isUsingBar;
        }

        public override string ToString()
        {
            return "Player: " + Player.EntityId;
        }
    }
}
