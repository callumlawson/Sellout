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

        public PlayerState(Entity player, bool tutorialControlLock)
        {
            Player = player;
            TutorialControlLock = tutorialControlLock;
        }

        public override string ToString()
        {
            return "Player: " + Player.EntityId;
        }
    }
}
