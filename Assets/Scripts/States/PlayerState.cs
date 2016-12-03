using System;
using Assets.Framework.Entities;
using Assets.Framework.States;
namespace Assets.Scripts.States
{
    [Serializable]
    class PlayerState : IState
    {
        public readonly Entity Player;

        public PlayerState(Entity player)
        {
            Player = player;
        }

        public override string ToString()
        {
            return "Player: " + Player.EntityId;
        }
    }
}
