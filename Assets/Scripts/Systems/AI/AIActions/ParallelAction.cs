using Assets.Framework.Entities;

namespace Assets.Scripts.Systems.AI.AIActions
{
    internal abstract class ParallelAction : CompositeAction
    {
        public override void OnStart(Entity entity)
        {
            Actions.ForEach(action => action.OnStart(entity));
        }

        public override void OnFrame(Entity entity)
        {
            Actions.ForEach(action => action.OnFrame(entity));
            Actions.RemoveAll(action => action.IsComplete);
        }
    }
}
