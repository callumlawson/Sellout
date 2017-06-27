using Assets.Framework.Entities;
using Assets.Scripts.GameActions.Framework;

namespace Assets.Scripts.Util
{
    public class EntityActionPair
    {
        private Entity entity;
        private GameAction gameAction;

        public EntityActionPair(Entity entity, GameAction gameAction)
        {
            this.entity = entity;
            this.gameAction = gameAction;
        }

        public Entity GetEntity()
        {
            return entity;
        }

        public GameAction GetGameAction()
        {
            return gameAction;
        }
    }
}
