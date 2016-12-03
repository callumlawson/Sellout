using System.Linq;
using Assets.Framework.Entities;
using Assets.Scripts.GameActions.Framework;

namespace Assets.Scripts.GameActions.Composite
{
    //Not tested yet!
    internal abstract class ParallelAction : CompositeAction
    {
        public override void OnStart(Entity entity)
        {
            Actions.ForEach(action =>
            {
                action.OnStart(entity);
                action.ActionStatus = ActionStatus.Running;
            });
        }

        public override void OnFrame(Entity entity)
        {
            Actions.ForEach(action => action.OnFrame(entity));
            Actions.RemoveAll(action => action.IsComplete());

            if (!Actions.Any())
            {
                ActionStatus = ActionStatus.Succeeded;
            }
        }
    }
}
