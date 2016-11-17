using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Framework.States;

namespace Assets.Framework.Entities
{
    [Serializable]
    public class Entity
    {
        private readonly EntityManager entityManager;
        public int EntityId { get; private set; }

        //For debugging only!
        public IEnumerable<IState> DebugStates
        {
            get { return States(); }
        }

        public Entity(EntityManager entityManager, int entityId)
        {
            this.entityManager = entityManager;
            EntityId = entityId;
        }

        public T GetState<T>() where T : IState
        {
            var state =  entityManager.GetState<T>(this);
            if (state == null)
            {
                UnityEngine.Debug.LogError(string.Format("State {0} missing for entity \n {1}", typeof(T), this));
            }
            return state;
        }

        public bool HasState(Type stateType)
        {
            return entityManager.GetState(this, stateType) != null;
        }

        public bool HasState<T>() where T : IState
        {
            return entityManager.GetState<T>(this) != null;
        }

        private IEnumerable<IState> States()
        {
           return entityManager.GetStates(this);
        }

        public override string ToString()
        {
            var message = new StringBuilder();
            message.AppendLine("EntityID: " + EntityId);
            if (DebugStates.ToList().Count == 0)
            {
                message.AppendLine("An entity with no states.");
            }
            foreach (var state in DebugStates)
            {
                message.AppendLine(state.ToString());
            }
            return message.ToString();
        }

        public override bool Equals(object obj)
        {
            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to Point return false.
            var otherEntity = obj as Entity;
            return otherEntity != null && Equals(otherEntity);
        }

        private bool Equals(Entity other)
        {
            return EntityId == other.EntityId;
        }

        public override int GetHashCode()
        {
            return EntityId;
        }
    }
}
