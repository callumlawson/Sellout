using Assets.Framework.Entities;
using Assets.Framework.Systems;
using Assets.Scripts.GameActions.Framework;
using Assets.Scripts.States;

namespace Assets.Scripts.GameActions
{
    public class DestoryEntityInInventoryAction : GameAction
    {
        public override void OnStart(Entity entity)
        {
            var inventoryItem = entity.GetState<InventoryState>().child;
            if (inventoryItem != null)
            {
                entity.GetState<InventoryState>().RemoveChild();
                EntityStateSystem.Instance.RemoveEntity(inventoryItem);
            }
            ActionStatus = ActionStatus.Succeeded;
        }

        public override void OnFrame(Entity entity)
        {
            //Nothing doing.
        }
    }
}
