using System.Linq;
using Assets.Framework.Entities;

namespace Assets.Scripts.Systems.AI.AIActions
{
    class ActionSequence : CompositeAction
    {
        public override void OnStart(Entity entity)
        {
            if (Actions.Any())
            {
                if (!Actions[0].IsStarted)
                {
                    Actions[0].OnStart(entity);
                    Actions[0].IsStarted = true;
                }
            }
        }

        public override void OnFrame(Entity entity)
        {
            if (Actions.Any())
            {
                if (!Actions[0].IsStarted)
                {
                    Actions[0].OnStart(entity);
                    Actions[0].IsStarted = true;
                }

                Actions[0].OnFrame(entity);
                if (Actions[0].IsComplete)
                {
                    Actions.RemoveAt(0);
                    if (Actions.Any())
                    {
                        Actions[0].OnStart(entity);
                    }
                }
            }
            IsComplete = !Actions.Any();
        }
    }
}
