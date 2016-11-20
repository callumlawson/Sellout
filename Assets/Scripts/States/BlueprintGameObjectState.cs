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
    }
}
