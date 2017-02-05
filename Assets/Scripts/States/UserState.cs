using System;
using Assets.Framework.Entities;
using Assets.Framework.States;

namespace Assets.Scripts.States
{
    [Serializable]
    class UserState : IState
    {
        public Entity Reserver;
        private string reserveReason = "";
        public Entity User;
        private string useReason = "";

        public bool IsFree()
        {
            return Reserver == null && User == null;
        }

        public void Reserve(Entity entity, string reason)
        {
            Reserver = entity;
            reserveReason = reason;
        }

        public void Use(Entity entity, string reason)
        {
            User = entity;
            useReason = reason;
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

        public void ClearUser()
        {
            User = null;
            useReason = "";
        }

        public void ClearReserver()
        {
            Reserver = null;
            reserveReason = "";
        }

        public void Free()
        {
            Reserver = null;
            User = null;
        }

        public override string ToString()
        {
            var reserved = Reserver != null
                ? string.Format("Reserved by: {0} for {1}", Reserver.EntityId, reserveReason)
                : "Reserved by: Nobody";
            var used = User != null ? string.Format("Used by: {0} for {1}", User.EntityId, useReason) : "Used by: Nobody";
            return reserved + " " + used;
        }
    }
}
