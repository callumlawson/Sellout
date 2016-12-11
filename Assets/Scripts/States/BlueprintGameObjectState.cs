using Assets.Framework.States;
using UnityEngine;

namespace Assets.Scripts.States
{
    //Init only. Mad Hax.
    class BlueprintGameObjectState : IState
    {
        public GameObject BlueprintGameObject;

        public BlueprintGameObjectState(GameObject blueprintGameObject)
        {
            BlueprintGameObject = blueprintGameObject;
        }

        public override string ToString()
        {
            return "You should not see this at runtime.";
        }
    }
}
