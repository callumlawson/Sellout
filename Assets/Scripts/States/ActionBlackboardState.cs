using System;
using System.Text;
using Assets.Framework.Entities;
using Assets.Framework.States;

namespace Assets.Scripts.States
{
    [Serializable]
    class ActionBlackboardState : IState
    {
        public Entity TargetWaypoint;
        public string CurrentActions;

        public ActionBlackboardState(Entity targetWaypoint)
        {
            TargetWaypoint = targetWaypoint;
        }

        public override string ToString()
        {
            var message = new StringBuilder();
            message.Append("AI: \n");
            message.Append(CurrentActions);
            return message.ToString();
        }
    }
}
