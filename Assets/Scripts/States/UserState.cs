using System;
using Assets.Framework.Entities;
using Assets.Framework.States;

namespace Assets.Scripts.States
{
    [Serializable]
    class UserState : IState
    {
        public Entity User;

        public UserState(Entity user)
        {
            User = user;
        }

        public bool IsFree()
        {
            return User == null;
        }

        public bool IsOccupied()
        {
            return !IsFree();
        }

        public override string ToString()
        {
            if (User != null)
            {
                return string.Format("Current User: {0}", User.EntityId);
            }
            else
            {
                return "No User";
            }
        }
    }
}
