using Assets.Framework.Entities;
using Assets.Framework.Systems;
using Assets.Scripts.States;

namespace Assets.Scripts.Systems.AI.AIActions
{
    public class DestoryEntityInInventoryAction : GameAction
    {
        private readonly Entity entityToRemove;

        public DestoryEntityInInventoryAction(Entity entity)
        {
            entityToRemove = entity;
        }

        public override void OnStart(Entity entity)
        {
            if (entityToRemove != null)
            {
                entity.GetState<InventoryState>().RemoveChild();
                EntityStateSystem.Instance.RemoveEntity(entityToRemove);
            }
            IsComplete = true;
        }

        public override void OnFrame(Entity entity)
        {
        }
    }
}
