using System;
using Assets.Framework.Entities;
using Assets.Framework.States;
using UnityEngine;

namespace Assets.Scripts.States
{
    [Serializable]
    public class SelectedState : IState
    {
        public Entity SelectedEntity;

        public SelectedState(Entity selectedEntity)
        {
        }
    }
}
