using System.Collections.Generic;
using Assets.Framework.States;

namespace Assets.Scripts.Blueprints
{
    public interface IEntityBlueprint
    {
        List<IState> EntityToSpawn();
    }
}
