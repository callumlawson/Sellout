using System;
using Assets.Framework.Entities;
using Assets.Framework.States;

namespace Assets.Scripts.States
{
    [Serializable]
    class UserState : IState
    {
        public Entity Reserver;
        public Entity User;

        public bool IsFree()
        {
            return Reserver == null && User == null;
        }

        public bool IsOccupied()
        {
            return !IsFree();
        }

        public bool IsInUse()
        {
            return User != null;
        }

        public bool IsReserved()
        {
            return Reserver != null;
        }

        public override string ToString()
        {
            var reserved = Reserver != null
                ? string.Format("Reserved by: {0}", Reserver.EntityId)
                : "Reserved by: Nobody";
            var used = User != null ? string.Format("Used by: {0}", User.EntityId) : "Used by: Nobody";
            return reserved + " " + used;
        }
    }
}
