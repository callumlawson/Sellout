using System;

namespace Assets.Framework.States
{
    [Serializable]
    class PrefabState : IState
    {
        public readonly string PrefabName;

        public PrefabState(string prefabName)
        {
            PrefabName = prefabName;
        }
    }
}
